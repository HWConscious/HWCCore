using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using HWC.DataModel;

using Amazon;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

using Npgsql;

/// <summary>
/// Namespace for Cloud Services
/// </summary>
namespace HWC.CloudService
{
    #region Configuration

    /// <summary>
    /// Cloud service configurations
    /// </summary>
    public class Config
    {
        /// <summary>
        /// Default AWS RegionEndpoint
        /// </summary>
        protected internal static RegionEndpoint AwsRegionEndpoint = RegionEndpoint.USEast1;

        /// <summary>
        /// Timeout threshold for DisplaySession's touch event. Value in seconds.
        /// </summary>
        public static int DisplaySessionTouchTimeoutThreshold = 5;

        /// <summary>
        /// Timeout threshold for DisplaySession's LocationDevice registration. Value in seconds.
        /// </summary>
        public static int DisplaySessionLocationDeviceTimeoutThreshold = 5;

        /// <summary>
        /// Timeout threshold for UserSession in a Zone. Value in seconds.
        /// </summary>
        public static int UserSessionZoneTimeoutThreshold = 12;

        /// <summary>
        /// Representation of DataClient configuration
        /// </summary>
        public class DataClientConfig
        {
            #region Data members

            public enum RdsDbInfrastructure
            {
                None = 0,
                Local = 1,
                Aws = 2
            };

            public readonly RegionEndpoint AwsRegionEndpoint;
            public readonly RdsDbInfrastructure RdsInfrastructure;
            public readonly DbContextOptions<RdsDbContext> RdsOptions;
            public readonly AmazonDynamoDBClient NosqlClient;
            public readonly DynamoDBContextConfig NosqlConfig;

            #endregion

            #region Initialize

            /// <summary>
            /// Constructs DataClient Configuration
            /// </summary>
            /// <param name="rdsInfrastructure">Relational Database Infrastructure</param>
            public DataClientConfig(RdsDbInfrastructure rdsInfrastructure)
            {
                AwsRegionEndpoint = Config.AwsRegionEndpoint;
                RdsInfrastructure = rdsInfrastructure;

                string host = string.Empty;
                string username = string.Empty;
                string pwd = string.Empty;

                switch (RdsInfrastructure)
                {
                    case RdsDbInfrastructure.Local:
                        // Connection parameters
                        host = "localhost";
                        username = "postgres";
                        pwd = "pde12345";
                        break;
                    case RdsDbInfrastructure.Aws:
                        // Connection parameters
                        host = "postgresql1.czy5h30grgw8.us-east-1.rds.amazonaws.com";
                        username = "postgresql1_mu";
                        pwd = "pde12345";
                        break;
                }

                if (!string.IsNullOrEmpty(host))
                {
                    // PostgreSQL connection string
                    NpgsqlConnectionStringBuilder connectionStringBuilder = new NpgsqlConnectionStringBuilder()
                    {
                        Host = host,
                        Port = 5432,
                        Username = username,
                        Password = pwd,
                        Database = "postgresql1_hwc1",
                        SslMode = SslMode.Disable
                    };

                    DbContextOptionsBuilder<RdsDbContext> rdsOptionsBuilder = new DbContextOptionsBuilder<RdsDbContext>();
                    rdsOptionsBuilder.UseNpgsql(connectionStringBuilder.ToString());
                    RdsOptions = rdsOptionsBuilder.Options;
                }
                
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(DisplayConcurrentList)] = new Amazon.Util.TypeMapping(typeof(DisplayConcurrentList), typeof(DisplayConcurrentList).Name);
                AWSConfigsDynamoDB.Context.TypeMappings[typeof(ZoneConcurrentList)] = new Amazon.Util.TypeMapping(typeof(ZoneConcurrentList), typeof(ZoneConcurrentList).Name);
                NosqlClient = new AmazonDynamoDBClient(AwsRegionEndpoint);
                NosqlConfig = new DynamoDBContextConfig { Conversion = DynamoDBEntryConversion.V2 };
            }

            #endregion
        }

        /// <summary>
        /// Representation of RestClient configuration
        /// </summary>
        public class RestClientConfig
        {
            #region Data members

            public readonly string RestApiEndpoint;
            public readonly string XApiKeyValue;

            #endregion

            #region Initialize

            /// <summary>
            /// Constructs RestClient Configuration
            /// </summary>
            public RestClientConfig(string xApiKeyValue)
            {
                if (xApiKeyValue != null)
                {
                    RestApiEndpoint = "https://oz3yqvjaik.execute-api.us-east-1.amazonaws.com/v1";
                    XApiKeyValue = xApiKeyValue;
                }
                else
                {
                    throw new ArgumentNullException("xApiKeyValue");
                }
            }

            #endregion
        }
    }

    #endregion

    #region Services

    /// <summary>
    /// Data provider service
    /// </summary>
    public class DataClient : IDisposable
    {
        #region Data members

        /// <summary>
        /// DataClient configuration
        /// </summary>
        public readonly Config.DataClientConfig DataClientConfig;

        private ConfigurationData _configurationData = null;
        /// <summary>
        /// Configuration data
        /// </summary>
        public ConfigurationData ConfigurationData
        {
            get
            {
                if (_configurationData == null)
                {
                    try
                    {
                        _configurationData = new ConfigurationData(DataClientConfig.RdsOptions);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to initiate Configuration Data", ex);
                    }
                }
                return _configurationData;
            }
        }

        private TransactionalData _transactionalData = null;
        /// <summary>
        /// Transactional data
        /// </summary>
        public TransactionalData TransactionalData
        {
            get
            {
                if (_transactionalData == null)
                {
                    try
                    {
                        _transactionalData = new TransactionalData(DataClientConfig.RdsOptions);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to initiate Transactional Data", ex);
                    }
                }
                return _transactionalData;
            }
        }

        private TransientData _transientData = null;
        /// <summary>
        /// Transient data
        /// </summary>
        public TransientData TransientData
        {
            get
            {
                if (_transientData == null)
                {
                    try
                    {
                        _transientData = new TransientData(DataClientConfig.NosqlClient, DataClientConfig.NosqlConfig);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Unable to initiate Transient Data", ex);
                    }
                }
                return _transientData;
            }
        }

        #endregion

        #region Initialize

        /// <summary>
        /// Constructs Data client
        /// </summary>
        /// <param name="dataClientConfig">DataClient configuration</param>
        public DataClient(Config.DataClientConfig dataClientConfig)
        {
            if (dataClientConfig != null)
            {
                DataClientConfig = dataClientConfig;
            }
            else
            {
                throw new ArgumentNullException("dataClientConfig");
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Dispose DataClient
        /// </summary>
        public void Dispose()
        {
            if (_configurationData != null)
            {
                _configurationData.Dispose();
                ConfigurationData.Dispose();
            }
            if (_transactionalData != null)
            {
                _transactionalData.Dispose();
                TransactionalData.Dispose();
            }
            if (_transientData != null)
            {
                _transientData.Dispose();
                TransientData.Dispose();
            }
        }

        #endregion
    }

    /// <summary>
    /// REST client service
    /// </summary>
    public class RestClient
    {
        #region Data members

        private readonly HttpVerb _httpMethod;
        private readonly Uri _endPoint;
        private readonly Dictionary<string, string> _headers;
        private readonly string _contentType;
        private string _content { get; set; }
        private int? _timeoutInMs { get; set; }

        /// <summary>
        /// REST request methods.
        /// </summary>
        public enum HttpVerb
        {
            GET,
            POST,
            PUT,
            DELETE
        }

        /// <summary>
        /// RestClient configuration
        /// </summary>
        public readonly Config.RestClientConfig RestClientConfig;

        #endregion

        #region Initialize

        /// <summary>
        /// Constructs REST client
        /// </summary>
        /// <param name="restClientConfig">Configuration for REST client</param>
        /// <param name="httpMethod">Method to be used for REST call</param>
        /// <param name="apiPath">REST API path. It should start with a Slash (/). E.g., /users</param>
        /// <param name="headers">HTTP request headers</param>
        /// <param name="contentType">Request content type. Default: "application/json"</param>
        /// <param name="content">Request content</param>
        /// <param name="timeoutInMs">Request timeout value in millisecond</param>
        public RestClient(Config.RestClientConfig restClientConfig, HttpVerb httpMethod, string apiPath, Dictionary<string, string> headers = null, string contentType = null, string content = null, int? timeoutInMs = null)
        {
            if (restClientConfig != null)
            {
                Dictionary<string, string> requestHeaders = null;
                var apiKeyName = "x-api-key";
                if (headers == null)
                {
                    requestHeaders = new Dictionary<string, string>() { { apiKeyName, restClientConfig.XApiKeyValue } };
                }
                else
                {
                    if (!headers.ContainsKey(apiKeyName))
                    {
                        headers.Add(apiKeyName, restClientConfig.XApiKeyValue);
                    }
                    requestHeaders = headers;
                }

                RestClientConfig = restClientConfig;
                _httpMethod = httpMethod;
                _endPoint = new Uri(restClientConfig.RestApiEndpoint + apiPath, UriKind.Absolute);
                _headers = requestHeaders;
                _contentType = contentType ?? "application/json";
                _content = content ?? string.Empty;
                _timeoutInMs = timeoutInMs;
            }
            else
            {
                throw new ArgumentNullException("restClientConfig");
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Makes RESTful HTTP request
        /// </summary>
        /// <returns>Response of the request (could be JSON, XML or HTML etc. serialized into string)</returns>
        public async Task<string> MakeRequestAsync()
        {
            HttpWebRequest webRequest = null;

            // Create a HttpWebRequest instance for web request
            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create(_endPoint);
                webRequest.Method = _httpMethod.ToString();
                webRequest.ContentType = _contentType;
                if (_headers?.Count > 0)
                {
                    foreach (KeyValuePair<string, string> header in _headers)
                    {
                        webRequest.Headers[header.Key] = header.Value;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to create HttpWebRequest handler. " + ex.Message);
            }

            if (webRequest != null)
            {
                return await ProcessWebRequestAsync(webRequest);
            }

            return null;
        }

        /// <summary>
        /// Updates request content
        /// </summary>
        /// <param name="content">Request content</param>
        /// <returns></returns>
        public bool UpdateContent(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                _content = content;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates request timeout
        /// </summary>
        /// <param name="timeoutInMs">Timeout value in millisecond</param>
        /// <returns></returns>
        public bool UpdateTimeout(int? timeoutInMs)
        {
            if (timeoutInMs != null && timeoutInMs < 0)
            {
                return false;
            }
            _timeoutInMs = timeoutInMs;
            return true;
        }

        /// <summary>
        /// Gets Endpoint of the REST client
        /// </summary>
        /// <returns></returns>
        public string GetEndpoint()
        {
            return _endPoint.AbsolutePath;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Processes HTTP web request
        /// </summary>
        /// <param name="webRequest"></param>
        /// <returns>Response of the request</returns>
        private async Task<string> ProcessWebRequestAsync(HttpWebRequest webRequest)
        {
            string responseValue = null;

            if (webRequest != null)
            {
                if (_httpMethod != HttpVerb.GET)
                {
                    // Write request content into web request stream
                    try
                    {
                        using (Stream requestStream = await webRequest.GetRequestStreamAsync())
                        {
                            byte[] contentInBytes = Encoding.UTF8.GetBytes(_content);
                            requestStream.Write(contentInBytes, 0, contentInBytes.Length);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("Error in request content. " + ex.Message);
                    }
                }

                // Make the web request and process the response
                HttpWebResponse webResponse = null;
                try
                {
                    // Set a timer to abort the web requset on timeout (if any)
                    CancellationTokenSource abortWebRequestTaskCancellationTokenSource = null;
                    if (_timeoutInMs != null && _timeoutInMs > -1)
                    {
                        abortWebRequestTaskCancellationTokenSource = new CancellationTokenSource();
                        AbortWebRequestOnTimeoutAsync(webRequest, abortWebRequestTaskCancellationTokenSource.Token);
                    }

                    // Make the web request
                    webResponse = (HttpWebResponse)await webRequest.GetResponseAsync();

                    // Cancel the task for aborting web request now as it made the full cycle before the timeout timer got hit
                    abortWebRequestTaskCancellationTokenSource?.Cancel();

                    // Process the response stream
                    using (Stream webResponseStream = webResponse.GetResponseStream())
                    {
                        if (webResponseStream != null)
                        {
                            // Read the response stream into string
                            using (StreamReader reader = new StreamReader(webResponseStream))
                            {
                                responseValue = await reader.ReadToEndAsync();
                            }
                        }
                    }
                }
                catch (WebException ex)
                {
                    // Handle different web exceptions
                    if (ex.Status == WebExceptionStatus.ProtocolError)
                    {
                        var statusCode = ((HttpWebResponse)ex.Response).StatusCode;
                        if (statusCode != HttpStatusCode.OK)
                        {
                            // Throw http status code
                            throw new Exception(((int)statusCode).ToString());
                        }
                    }
                    else if (ex.Status == WebExceptionStatus.RequestCanceled)
                    {
                        throw new Exception("Web request was timed out, hence aborted.");
                    }
                    else
                    {
                        throw new Exception("Error in RESTful HTTP request (" + ex.Status.ToString() + "). " + ex.Message);
                    }
                }

                webResponse?.Dispose();
            }

            return responseValue;
        }

        /// <summary>
        /// Sets a timer to abort the web requset on timeout (if any)
        /// </summary>
        /// <param name="webRequest"></param>
        /// <param name="cancellationToken"></param>
        private async void AbortWebRequestOnTimeoutAsync(HttpWebRequest webRequest, CancellationToken cancellationToken)
        {
            if (webRequest != null)
            {
                await Task.Run(async () =>
                {
                    await Task.Delay(_timeoutInMs ?? 0); // Set timeout timer
                    if (!cancellationToken.IsCancellationRequested)
                    {
                        webRequest.Abort(); // Abort the web request
                    }
                }, cancellationToken); // Set the timeout Task with cancellation token
            }
        }

        #endregion
    }

    #endregion
}
