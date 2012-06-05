using System;
using System.Runtime.Serialization;
using GameOfLife.WorldEditing;

namespace GameOfLife.GameLogic {
    [DataContract]
    [Editable("aaaaa")]
    public class Career {
        /// <summary>
        /// Stores the type of Career that this card is associated with. IE CollgeCareer.
        /// </summary>
        [DataMember]
        [Editable("Required Career")]
        public CareerType CareerType { get; set; }

        /// <summary>
        /// The name associated with this career. IE. "Jet Pilot"
        /// </summary>
        [DataMember]
        [Editable("Name")]
        public String Title { get; set; }

        /// <summary>
        /// The salarys associated with a career.
        /// Ie salary[0] = start wage,
        ///    salary[n] = promotion wage etc.
        /// </summary>
        [DataMember]
        [Editable("Salary")]
        public int[] Salary { get; private set; }

        public Career(CareerType careerType, String careerName, int[] salary) {
            CareerType = careerType;
            Title = careerName;
            Salary = salary;
        }

        public override string ToString() {
            return Title;
        }
    }
}