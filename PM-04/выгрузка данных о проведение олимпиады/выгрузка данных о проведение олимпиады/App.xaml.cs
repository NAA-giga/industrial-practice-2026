using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Windows;
using выгрузка_данных_о_проведение_олимпиады.Models;
using выгрузка_данных_о_проведение_олимпиады.Services;
using выгрузка_данных_о_проведение_олимпиады.ViewModels;
using выгрузка_данных_о_проведение_олимпиады.Views;

namespace выгрузка_данных_о_проведение_олимпиады
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private IHost _host = null!;

         protected override void OnStartup(StartupEventArgs e)
         {
             // 1. Создаём хост с настройками по умолчанию
             _host = Host.CreateDefaultBuilder(e.Args)
                 .ConfigureAppConfiguration((context, config) =>
                 {
                     // Добавляем JSON-файл конфигурации (опционально, CreateDefaultBuilder уже добавляет appsettings.json)
                     config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                 })
                 .ConfigureServices((context, services) =>
                 {
                     // 2. Регистрируем DbContext
                     services.AddDbContext<School_OlympiadDBContext>(options =>
                         options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

                     // 3. Регистрируем сервисы
                     services.AddScoped<IOlympiadDataService, OlympiadDataService>();
                     services.AddSingleton<IDialogService, DialogService>();
                     services.AddSingleton<IExcelService, ExcelService>();

                     // 4. Регистрируем ViewModels
                     services.AddTransient<MainViewModel>();

                     // 5. Регистрируем окна
                     services.AddSingleton<MainWindow>();
                 })
                 .Build();

             // 6. Получаем главное окно из контейнера и показываем
             var mainWindow = _host.Services.GetRequiredService<MainWindow>();
             mainWindow.DataContext = _host.Services.GetRequiredService<MainViewModel>();
             mainWindow.Show();

             base.OnStartup(e);
         }

         protected override void OnExit(ExitEventArgs e)
         {
             // 7. Корректно освобождаем ресурсы хоста
             _host?.Dispose();
             base.OnExit(e);
         }
        
    }

}
