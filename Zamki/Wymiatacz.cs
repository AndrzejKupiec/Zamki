using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Zamki
{
    class Wymiatacz
    {

        private int id;
        private byte[] adres = new byte[8] ;
        private bool state;

        public int Id
        {
            get
            {
                return id;
            }

            set
            {
                id = value;
            }
        }

        public byte[] Adres
        {
            get
            {
                return adres;
            }

            set
            {
                adres = value;
            }
        }

        public bool State
        {
            get
            {
                return state;
            }

            set
            {
                state = value;
            }
        }

        public Wymiatacz(int nId, byte[] nAdres, bool nState )
        {
            id = nId;
            for (int i = 0; i < 8; i++) { adres[i] = nAdres[i]; };
            state = nState;
        }


    }
}
