using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TronServerNeu
{
    class FreeIDs
    {
        private Stack<byte> freieIDs;

        public FreeIDs() {
            freieIDs = new Stack<byte>();
            freieIDs.Push(0);
        }

        public void Push(byte ID)
        {
            freieIDs.Push(ID);
        }
        
        public byte Pop()
        {
            if(freieIDs.Count > 1)
            {
                return freieIDs.Pop();
            }

            byte toRet = freieIDs.Pop();
            try
            {
                freieIDs.Push((byte)(toRet + 1));
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("Zu viele Spieler byte reicht nicht");
            }
            return toRet;
        }

        public byte Peek()
        {
            return freieIDs.Peek();
        }

        public void Reorganise()
        {
            //https://dotnetfiddle.net/4lkxg3

            Stack<byte> temp = new Stack<byte> { };

            while (freieIDs.Count > 0)
            {

                byte t = freieIDs.Pop();

                while (temp.Count > 0 && t < temp.Peek())
                {

                    freieIDs.Push(temp.Pop());
                }

                temp.Push(t);
            }

            freieIDs = temp;

            //löschung unnötiger IDs

            byte vergleichswert = freieIDs.Pop();
            while (freieIDs.Count > 1 && vergleichswert - 1 == freieIDs.Peek())
            {
                freieIDs.Pop();
            }
            if (freieIDs.Count == 0) {
                Push(0);
            }
            else {
                freieIDs.Push(vergleichswert);

            }
            //umdrehen
        }
    }
}

