using DashboardData.Models;
namespace DashboardData.Services
{
    public interface ISensorService
    {
        Task<List<SensorData>> GetSensorsAsync();
        Task<List<Location>> GetLocationsAsync();
        Task<SensorData> GetSensorByIdAsync(int id);
        Task ReloadSensorAsync(SensorData sensor);
        Task AddSensorAsync(SensorData sensor);
        Task UpdateSensorAsync(SensorData sensor);
        Task DeleteSensorAsync(int id);
        Task<List<SensorData>> GetCriticalSensorsAsync(double threshold);
        Task<int> GetTotalCountAsync();
        Task<double> GetAverageValueAsync();
        Task<double> GetMaxValueAsync();
        Task<List<LocationStat>> GetAverageValueByLocationAsync();

    }
}
