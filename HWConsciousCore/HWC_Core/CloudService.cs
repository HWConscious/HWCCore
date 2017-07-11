using System;
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
        // Default AWS RegionEndpoint
        protected internal static RegionEndpoint AwsRegionEndpoint = RegionEndpoint.USEast1;

        // Timeout threshold for DisplaySession touch event. Value in seconds.
        public static int DisplaySessionTouchTimeoutThreshold = 5;

        // Timeout threshold for UserSession in a Zone. Value in seconds.
        public static int UserSessionZoneTimeoutThreshold = 5;

        /// <summary>
        /// Representation of DataClient configuration
        /// </summary>
        public class DataClientConfig
        {
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

            /// <summary>
            /// Constructor
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
        }
    }

    #endregion

    #region Services

    /// <summary>
    /// Data provider service
    /// </summary>
    public class DataClient : IDisposable
    {
        public readonly Config.DataClientConfig DataClientConfig;

        private ConfigurationData _configurationData = null;
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

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="dataClientConfig">DataClient configuration</param>
        public DataClient(Config.DataClientConfig dataClientConfig)
        {
            DataClientConfig = dataClientConfig;
        }

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
    }

    #endregion
}
