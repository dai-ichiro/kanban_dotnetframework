using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MyWinFormsApp
{
    public class HospitalData
    {
        private readonly List<Hospital> _hospitals;

        private HospitalData(List<Hospital> hospitals)
        {
            _hospitals = hospitals;
        }

        public static HospitalData Load()
        {
            // Use local directory instead of BaseDirectory for better behavior in some environments
            var yamlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "dialysis_facility.yaml");
            if (!File.Exists(yamlPath))
            {
                // Fallback to project directory if not found in BaseDirectory (during debugging)
                yamlPath = "dialysis_facility.yaml";
            }

            if (!File.Exists(yamlPath))
                throw new FileNotFoundException("dialysis_facility.yaml not found", yamlPath);

            var lines = File.ReadAllLines(yamlPath);
            var hospitals = new List<Hospital>();
            Hospital current = null;
            foreach (var line in lines)
            {
                var trimmed = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                    continue;
                if (trimmed.StartsWith("- name:"))
                {
                    var name = trimmed.Substring(7).Trim();
                    current = new Hospital { Name = name };
                    hospitals.Add(current);
                }
            }
            return new HospitalData(hospitals);
        }

        public IEnumerable<string> GetAll()
        {
            return _hospitals.Select(h => h.Name);
        }

        private class Hospital
        {
            public string Name { get; set; }
        }
    }
}
