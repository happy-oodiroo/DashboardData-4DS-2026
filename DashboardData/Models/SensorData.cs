using System.ComponentModel.DataAnnotations;

namespace DashboardData.Models
{
    public class SensorData    {

        [Key]
        public int Id { get; set; }
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 50 characters.")]
        public string Name { get; set; }

        public string Type { get; set; } = "Temperature";
        [Range(-50.0, 150.0, ErrorMessage = "Value must be between -50.0 and 150.0.")]
        public double Value { get; set; }

        public DateTime LastUpdate { get; set; } = DateTime.Now;


        //====== Entity Framework Core relationships ======

        // Foreign key to Location (1-to-N): 1 sensor belongs to 1 location
        [Range(1, int.MaxValue, ErrorMessage = "You must select a location.")]
        public int LocationId { get; set; }  
        public Location Location { get; set; }

        // Many-to-Many relationship with Tag: 1 sensor can have multiple tags, and 1 tag can be associated with multiple sensors
        public ICollection<Tag> Tags { get; set; } = new List<Tag>();  

        // One-to-Many relationship with SensorValueHistor: 1 sensor can have multiple historical values
        public ICollection<SensorValueHistory> SensorValueHistories { get; set; } = new List<SensorValueHistory>();
    }

    public class LocationStat
    {
        public string LocationName { get; set; }
        public double AverageValue { get; set; }
    }
}
