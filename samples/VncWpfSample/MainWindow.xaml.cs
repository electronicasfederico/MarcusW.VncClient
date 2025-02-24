using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MarcusW.VncClient;
using MarcusW.VncClient.Protocol.Implementation.Services.Transports;
using Microsoft.Extensions.Logging;

namespace VncWpfSample;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window, INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged(string propertyName) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    public RfbConnection _connection;
    private string interruptionCause;
    public string InterruptionCause
    {
        get => interruptionCause;
        set
        {
            if (interruptionCause != value)
            {
                interruptionCause = value;
                OnPropertyChanged(nameof(InterruptionCause));
            }
        }
    }
    
         private string connectionState;
    public string ConnectionState
    {
        get => connectionState;
        set
        {
            if (connectionState != value)
            {
                connectionState = value;
                OnPropertyChanged(nameof(ConnectionState));
            }
        }
    }

    public MainWindow()
    {
        InitializeComponent();
        this.DataContext = this;
        Loaded += MainWindow_Loaded;
    }

    private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
    {
        // Configuramos el logger y el cliente VNC
        var loggerFactory = new LoggerFactory();
        var vncClient = new VncClient(loggerFactory);
        var authHandler = new DemoAuthenticationHandler();

        // Parámetros de conexión (ajusta Host, Port y contraseña según corresponda)
        var parameters = new ConnectParameters
        {
            TransportParameters = new TcpTransportParameters
            {
                Host = "127.0.0.1", // Dirección IP o nombre del servidor VNC
                Port = 5900            // Puerto de conexión
            },
            AuthenticationHandler = authHandler
        };

        try
        {
            _connection = await vncClient.ConnectAsync(parameters, CancellationToken.None).ConfigureAwait(false);
            _connection.PropertyChanged += Connection_PropertyChanged;

            // Asigna el render target en el hilo de la UI para que se muestre en el Image
            _ = Application.Current.Dispatcher.BeginInvoke(new Action(() => {
                _connection.RenderTarget = new WpfRenderTarget(VncImage);
            }));

        }
        catch (Exception ex)
        {
            MessageBox.Show("Error de conexión: " + ex.Message);
        }
    }

    private void Connection_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        InterruptionCause = _connection.InterruptionCause?.Message ?? string.Empty;
        ConnectionState = _connection.ConnectionState.ToString();
    }
}
