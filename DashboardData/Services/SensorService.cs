using DashboardData.Data;
using DashboardData.Models;
using Microsoft.EntityFrameworkCore;
namespace DashboardData.Services
{
    public class SensorService : ISensorService
    {
        private readonly AppDbContext _dbContext;
        public SensorService(AppDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<List<SensorData>> GetSensorsAsync()
        {
            // EF Core traduit Include par un JOIN SQL vers la table Location
            return await _dbContext.Sensors
                .Include(s => s.Location)
                .ToListAsync();
        }

        public async Task<List<Location>> GetLocationsAsync()
        {
            return await _dbContext.Locations.ToListAsync();
        }

        public async Task<SensorData> GetSensorByIdAsync(int id)
        {
            // FindAsync searches directly by Primary Key (Id)
            return await _dbContext.Sensors.FindAsync(id);
        }

        public async Task ReloadSensorAsync(SensorData sensor)
        {
            await _dbContext.Entry(sensor).ReloadAsync();
        }

        public async Task AddSensorAsync(SensorData sensor)
        {
            sensor.LastUpdate = DateTime.Now;
            
            // Archiving the initial value (from LAB 5)
            sensor.SensorValueHistories.Add(new SensorValueHistory {
                MeasuredValue = sensor.Value,
                Timestamp = DateTime.Now
            });

            _dbContext.Sensors.Add(sensor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateSensorAsync(SensorData sensor)
        {
            sensor.LastUpdate = DateTime.Now; // Update the date
            
            // Adding to the history upon modification (from LAB 5)
            sensor.SensorValueHistories.Add(new SensorValueHistory {
                MeasuredValue = sensor.Value,
                Timestamp = DateTime.Now
            });

            _dbContext.Sensors.Update(sensor);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteSensorAsync(int id)
        {
            var sensor = await _dbContext.Sensors.FindAsync(id);
            if (sensor != null)
            {
                _dbContext.Sensors.Remove(sensor);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<SensorData>> GetCriticalSensorsAsync(double threshold)
        {
            return await _dbContext.Sensors
                .Include(s => s.Location)
                .Where(s => s.Value > threshold) 
                .OrderByDescending(s => s.Value) 
                .ToListAsync();                  
        }

        public async Task<double> GetAverageValueAsync()
        {
            if (!await _dbContext.Sensors.AnyAsync()) return 0;

            return await _dbContext.Sensors.AverageAsync(s => s.Value);
        }

        public async Task<double> GetMaxValueAsync()
        {
            if (!await _dbContext.Sensors.AnyAsync()) return 0;
            return await _dbContext.Sensors.MaxAsync(s => s.Value);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _dbContext.Sensors.CountAsync();
        }

        public async Task<List<LocationStat>> GetAverageValueByLocationAsync()
        {
            // EF Core traduit ceci en : SELECT Location, AVG(Value) FROM Sensors GROUP BY Location
            return await _dbContext.Sensors
                .Include(s => s.Location)
                .GroupBy(s => s.Location.Name)
                .Select(g => new LocationStat 
                { 
                    LocationName = g.Key ?? "Inconnu", 
                    AverageValue = g.Average(s => s.Value) 
                })
                .ToListAsync();
        }
    }
}
