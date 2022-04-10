using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public abstract class Block
    {
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartOffSet { get; }
        public abstract int Id { get; }

        private int rotationState;
        private Position offset;

        public Block()
        { //gera o bloco 
            offset = new Position(StartOffSet.Row, StartOffSet.Column); 
        }

        public IEnumerable<Position> TilePositions()
        {
            foreach(Position p in Tiles[rotationState])
            {
                yield return new Position (p.Row + offset.Row, p.Column + offset.Column);
            }
        }

        public void RotateCW()
        { //faz o bloco girar para um lado
            rotationState = (rotationState + 1) % Tiles.Length;
        }

        public void RotateCCW()
        { // faz o bloco ir para lado oposto
            if (rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            }
            else
            {
                rotationState--;
            }
        }

        public void Move(int rows, int columns)
        { //Método que define o movimento do bloco
            offset.Row += rows;
            offset.Column += columns;
        }

        public void reset()
        { //metodo para resetar a posição inicial do bloco
            rotationState = 0;
            offset.Row = StartOffSet.Row;
            offset.Column = StartOffSet.Column;
        }
    }
}
