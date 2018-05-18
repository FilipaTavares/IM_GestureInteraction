using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppGui.Data
{
    class ParkData
    {
        private string nome;
        private int capacidade;
        private int ocupado;
        private int livre;
        private bool found;

        public ParkData(string nome, bool found)
        {
            this.nome = nome;
            this.found = found;
        }

        public string Nome
        {
            get
            {
                return nome;
            }

            set
            {
                nome = value;
            }
        }

        public int Capacidade
        {
            get
            {
                return capacidade;
            }

            set
            {
                capacidade = value;
            }
        }

        public int Ocupado
        {
            get
            {
                return ocupado;
            }

            set
            {
                ocupado = value;
            }
        }

        public int Livre
        {
            get
            {
                return livre;
            }

            set
            {
                livre = value;
            }
        }

        public bool Found
        {
            get
            {
                return found;
            }

            set
            {
                found = value;
            }
        }
    }
}
