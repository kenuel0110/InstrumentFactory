using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InstrumentFactory
{
    //
    public struct User
    {
        public byte[] image;
        public string fullname;
        public string number_work_treaty;
        public string date_work_treaty;
        public string birth_date;
        public string place_birth;
        public string citizenship;
        public string education;
        public string proffecion;
        public string num_departament;

        public User(
            byte[] image,
            string fullname,
            string number_work_treaty,
            string date_work_treaty,
            string birth_date,
            string place_birth,
            string citizenship,
            string education,
            string proffecion,
            string num_departament
            )
        {
            this.image = image;
            this.fullname = fullname;
            this.number_work_treaty = number_work_treaty;
            this.date_work_treaty = date_work_treaty;
            this.birth_date = birth_date;
            this.place_birth = place_birth;
            this.citizenship = citizenship;
            this.education = education;
            this.proffecion = proffecion;
            this.num_departament = num_departament;
        }
    }
}
