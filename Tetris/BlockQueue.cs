using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tetris
{
    public class BlockQueue
    {
        private readonly Block[] blocks = new Block[]
        { //cria todos os tipos de bloco
            new IBlock(),
            new JBlock(),
            new LBlock(),
            new OBlock(),
            new SBlock(),
            new TBlock(),
            new ZBlock()
        };

        private readonly Random random = new Random();

        public Block NextBlock { get; private set; }

        public BlockQueue()
        {  //chama a fila de blocos
            NextBlock = RandomBlock();
        }

        private Block RandomBlock()
        {//gera a fila aleatoria de blocos a nascerem
            return blocks[random.Next(blocks.Length)];
        }

        public Block GetAndUpdate()
        { // gera o novo bloco da fila
            Block block = NextBlock;

            do
            {
                NextBlock = RandomBlock();
            }
            while (block.Id == NextBlock.Id);

            return block;
        }
    }

}
