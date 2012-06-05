using System.Collections.Generic;
using GameOfLife.GameLogic;

namespace GameOfLife.Data {
    class CareerData {

        public static List<Career> PopulateCareers() {
            var possibleCareers = new List<Career> {
                // Career Types
                new Career(CareerType.Career, "Network Administrator", new [] { 90000, 150000, 200000, 250000}),
                new Career(CareerType.Career, "Cook", new [] {40000,50000, 150000, 800000}),
                new Career(CareerType.Career, "Cop", new[]{ 40000, 50000, 200000, 600000}),
                new Career(CareerType.Career, "Priest", new[]{ 60000, 100000, 150000, 200000}),
                new Career(CareerType.Career, "Painter", new[]{ 60000, 100000, 150000, 200000}),
                new Career(CareerType.Career, "Hairdresser", new[]{ 40000, 50000, 200000, 600000}),
                new Career(CareerType.Career, "Mechanic", new[]{ 60000, 100000, 150000, 200000}),
                new Career(CareerType.Career, "Photographer", new[]{ 60000, 100000, 150000, 200000}),

                // College careers
                new Career(CareerType.CollegeCareer, "Dentist", new [] { 70000, 130000, 160000, 200000}),
                new Career(CareerType.CollegeCareer, "Doctor", new [] { 50000, 150000, 250000, 450000}),
                new Career(CareerType.CollegeCareer, "Chemist", new [] { 50000, 80000, 100000, 150000}),
                new Career(CareerType.CollegeCareer, "Lawyer", new [] { 80000, 150000, 200000, 300000}),
                new Career(CareerType.CollegeCareer, "Pharmacist", new[]{ 40000, 50000, 200000, 600000}),
                new Career(CareerType.CollegeCareer, "Judge", new [] { 80000,100000, 150000, 200000}),
                new Career(CareerType.CollegeCareer, "Teacher", new[]{ 70000, 150000, 200000, 250000}),
                new Career(CareerType.CollegeCareer, "Surgeon", new[]{ 100000, 150000, 200000, 300000}),
                new Career(CareerType.CollegeCareer, "Vet", new[]{ 100000, 150000, 200000, 300000}),
                new Career(CareerType.CollegeCareer, "Financier", new[]{ 60000, 100000, 150000, 200000}),
                new Career(CareerType.CollegeCareer, "Nurse", new[]{ 40000, 50000, 200000, 600000}),
                new Career(CareerType.CollegeCareer, "Network Administrator", new [] { 60000, 100000, 150000, 200000})
            };
            return possibleCareers;
        }

    }
}
