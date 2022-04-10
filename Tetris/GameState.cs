using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class GameState  
    {
        private Block currentBlock;

        public Block CurrentBlock
        {
            get => currentBlock;
            private set
            {
                currentBlock = value;
                currentBlock.reset();

                for(int i = 0; i < 2; i++)
                {
                    currentBlock.Move(1, 0); //para que o bloco desça
                    if (!BlockFits())
                    {
                        currentBlock.Move(-1, 0); //
                    }
                }
            }
        }

        public GameGrid GameGrid { get; }
        public BlockQueue BlockQueue { get; }
        public bool GameOver { get; private set; }
        public int Score { get; private set; }
        public Block HeldBlock { get; private set; }
        public bool CanHold { get; private set; }
        public GameState()
        { //cria o grid, a pilha de blocos, o novo bloco e o que virá
            GameGrid = new GameGrid(22, 10);
            BlockQueue = new BlockQueue();
            CurrentBlock = BlockQueue.GetAndUpdate();
            CanHold = true;
        }

        private bool BlockFits()
        {
            foreach(Position p in CurrentBlock.TilePositions())
            {
                if (!GameGrid.IsEmpity(p.Row, p.Column))
                {
                    return false;
                }
            }

            return true;
        }

        public void HoldBlock()
        { // método para fazer a mecanica de segurar um bloco
            if (!CanHold)
            {
                return;
            }

            if(HeldBlock == null)
            { //se não há nenhum bloco segurado, a caixa fica vazia
                HeldBlock = CurrentBlock;
                CurrentBlock = BlockQueue.GetAndUpdate();
            }
            else 
            {
                Block tmp = CurrentBlock;
                CurrentBlock = HeldBlock;
                HeldBlock = tmp;
            }

            CanHold = false;
        }

        public void RotateBlockCW()
        { // Rotacionar o bloco para um lado
            CurrentBlock.RotateCW();

                if (!BlockFits())
            {
                CurrentBlock.RotateCCW();
            }
        }

        public void RotateBlockCCW()
        { //Rotacionar o bloco para o outro lado
            CurrentBlock.RotateCCW();

            if (!BlockFits())
            {
                CurrentBlock.RotateCW();
            }
        }

        public void MoveBlockLeft()
        { //Comando para fazer o bloco ir 1 unidade para a esquerda
            CurrentBlock.Move(0, -1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, 1);
            }
        }

        public void MoveBlockRIght()
        { //mover o bloco 1 unidade para a direita
            CurrentBlock.Move(0, 1);

            if (!BlockFits())
            {
                CurrentBlock.Move(0, -1);
            }
        }

        public bool IsGameOver()
        { //Verifica se o grid está vazio ou não, caso não tenha nenhum espaço vazio, será game over
            return !(GameGrid.IsRowEmpity(0) && GameGrid.IsRowEmpity(1));
        }

        private void PlaceBlock()
        { // Faz os blocos spawnarem e verifica se o jogo acabou ou não
            foreach (Position p in CurrentBlock.TilePositions())
            { 
                GameGrid[p.Row, p.Column] = CurrentBlock.Id;
            }

            Score += GameGrid.ClearFullRows(); //conta o score com as linhas eliminadas

            if (IsGameOver())
            {
                GameOver = true;
            }
            else
            {
                CurrentBlock = BlockQueue.GetAndUpdate(); // faz os blocos nascerem e se atualizarem
                CanHold = true;
            }
        }

        public void MoveBlockDown()
        { //comando para descer o bloco em uma unidade
            CurrentBlock.Move(1, 0);

            if (!BlockFits())
            {            
            CurrentBlock.Move(-1, 0);
            PlaceBlock();
            }
        }

        private int TileDropDistance(Position p)
        {
            int drop = 0;

            while(GameGrid.IsEmpity(p.Row + drop + 1, p.Column))
            {
                drop++;
            }

            return drop;
        }

        public int BlockDropDistance()
        {
            int drop = GameGrid.Rows;

            foreach(Position p in CurrentBlock.TilePositions())
            {
                drop = System.Math.Min(drop, TileDropDistance(p));
            }
            return drop;
        }

        public void DropBlock()
        {
            CurrentBlock.Move(BlockDropDistance(), 0);
            PlaceBlock();
        }
    }
}
