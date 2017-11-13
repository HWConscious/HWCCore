using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

using HWC.Core;

using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;

/// <summary>
/// Namespace for Data Models
/// </summary>
namespace HWC.DataModel
{
    #region Configuration Data Models

    [Table("Client")]
    public class Client
    {
        #region Database Properties

        [Key]
        public long ClientID { get; set; }                              // PRIMARY Key

        [Required]
        [Column("Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Column("Address")]
        [StringLength(50, MinimumLength = 3)]
        public string Address { get; set; }

        [Required]
        [Column("PhoneNumber")]
        [StringLength(50, MinimumLength = 3)]
        public string PhoneNumber { get; set; }

        #endregion

        #region Navigation Properties

        public List<ClientSpot> ClientSpots { get; set; }

        #endregion
    }

    [Table("ClientSpot")]
    public class ClientSpot
    {
        #region Database Properties

        [Key]
        public long ClientSpotID { get; set; }                          // PRIMARY Key

        [Required]
        [ForeignKey("FK_ClientSpot_Client_ClientID")]
        [Column("ClientID")]
        public long ClientID { get; set; }                              // FOREIGN Key to -> Client:ClientID

        [Required]
        [Column("Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Column("Address")]
        [StringLength(50, MinimumLength = 3)]
        public string Address { get; set; }

        [Required]
        [Column("PhoneNumber")]
        [StringLength(50, MinimumLength = 3)]
        public string PhoneNumber { get; set; }

        #endregion

        #region Navigation Properties

        public Client Client { get; set; }
        public List<Zone> Zones { get; set; }
        public List<LocationDevice> LocationDevices { get; set; }
        public List<DisplayEndpoint> DisplayEndpoints { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Coupon> Coupons { get; set; }

        #endregion
    }

    [Table("Zone")]
    public class Zone
    {
        #region Database Properties

        [Key]
        public long ZoneID { get; set; }                                // PRIMARY Key

        [Required]
        [ForeignKey("FK_Zone_ClientSpot_ClientSpotID")]
        [Column("ClientSpotID")]
        public long ClientSpotID { get; set; }                          // FOREIGN Key to -> ClientSpot:ClientSpotID

        [Required]
        [Column("Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        public ClientSpot ClientSpot { get; set; }
        public List<LocationDevice> LocationDevices { get; set; }
        public List<DisplayEndpoint> DisplayEndpoints { get; set; }

        #endregion
    }

    [Table("LocationDevice")]
    public class LocationDevice
    {
        #region Database Properties

        [Key]
        public long LocationDeviceID { get; set; }                      // PRIMARY Key

        [Required]
        [ForeignKey("FK_LocationDevice_ClientSpot_ClientSpotID")]
        [Column("ClientSpotID")]
        public long ClientSpotID { get; set; }                          // FOREIGN Key to -> ClientSpot:ClientSpotID

        [ForeignKey("FK_LocationDevice_Zone_ZoneID")]
        [Column("ZoneID")]
        public long? ZoneID { get; set; }                               // FOREIGN Key to -> Zone:ZoneID

        [Required]
        [EnumDataType(typeof(LocationDeviceType))]
        [Column("Type")]
        [Range(0, 1)]
        public LocationDeviceType Type { get; set; }

        [Required]
        [Column("DeviceID")]
        [StringLength(200, MinimumLength = 1)]
        public string DeviceID { get; set; }                            // UNIQUE Key

        #endregion

        #region Navigation Properties

        public ClientSpot ClientSpot { get; set; }
        public Zone Zone { get; set; }
        public List<LocationDeviceNotification> LocationDeviceNotifications { get; set; }

        #endregion
    }

    [Table("DisplayEndpoint")]
    public class DisplayEndpoint
    {
        #region Database Properties

        [Key]
        public long DisplayEndpointID { get; set; }                     // PRIMARY Key

        [Required]
        [ForeignKey("FK_DisplayEndpoint_ClientSpot_ClientSpotID")]
        [Column("ClientSpotID")]
        public long ClientSpotID { get; set; }                          // FOREIGN Key to -> ClientSpot:ClientSpotID

        [ForeignKey("FK_DisplayEndpoint_Zone_ZoneID")]
        [Column("ZoneID")]
        public long? ZoneID { get; set; }                               // FOREIGN Key to -> Zone:ZoneID

        [Required]
        [Column("Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        #endregion

        #region Navigation Properties

        public ClientSpot ClientSpot { get; set; }
        public Zone Zone { get; set; }
        public List<DisplayEndpointNotification> DisplayEndpointNotifications { get; set; }

        #endregion
    }

    [Table("Notification")]
    public class Notification
    {
        #region Database Properties

        [Key]
        public long NotificationID { get; set; }                        // PRIMARY Key

        [Required]
        [ForeignKey("FK_Notification_ClientSpot_ClientSpotID")]
        [Column("ClientSpotID")]
        public long ClientSpotID { get; set; }                          // FOREIGN Key to -> ClientSpot:ClientSpotID

        [Required]
        [Column("Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Column("SortOrder")]
        [Range(0, 50)]
        public int? SortOrder { get; set; }

        [Required]
        [Column("Timeout")]
        [Range(0, 3600)]
        public int Timeout { get; set; }

        [Required]
        [Column("Active")]
        public bool Active { get; set; }

        [Required]
        [Column("ShowProgressBar")]
        public bool ShowProgressBar { get; set; }

        [Required]
        [EnumDataType(typeof(MimeType))]
        [Column("ContentMimeType")]
        [Range(0, 3)]
        public MimeType ContentMimeType { get; set; }

        [Required]
        [Column("ContentSubject")]
        [StringLength(50, MinimumLength = 3)]
        public string ContentSubject { get; set; }

        [Column("ContentCaption")]
        [StringLength(200, MinimumLength = 3)]
        public string ContentCaption { get; set; }

        [Required]
        [Column("ContentBody")]
        [StringLength(200, MinimumLength = 1)]
        public string ContentBody { get; set; }

        #endregion

        #region Navigation Properties

        public ClientSpot ClientSpot { get; set; }
        public List<Coupon> Coupons { get; set; }
        public List<LocationDeviceNotification> LocationDeviceNotifications { get; set; }
        public List<DisplayEndpointNotification> DisplayEndpointNotifications { get; set; }

        #endregion

        #region Initialize

        public Notification()
        {
            // Set default values
            Active = true;
            ShowProgressBar = true;
        }

        #endregion
    }

    [Table("LocationDeviceNotification")]
    public class LocationDeviceNotification
    {
        #region Database Properties

        [Required]
        [ForeignKey("FK_LocationDeviceNotification_LocationDevice_LocationDeviceID")]
        [Column("LocationDeviceID")]
        public long LocationDeviceID { get; set; }                      // UNIQUE Key; FOREIGN Key to -> LocationDevice:LocationDeviceID

        [Required]
        [ForeignKey("FK_LocationDeviceNotification_Notification_NotificationID")]
        [Column("NotificationID")]
        public long NotificationID { get; set; }                        // UNIQUE Key; FOREIGN Key to -> Notification:NotificationID

        #endregion

        #region Navigation Properties

        public LocationDevice LocationDevice { get; set; }
        public Notification Notification { get; set; }

        #endregion
    }

    [Table("DisplayEndpointNotification")]
    public class DisplayEndpointNotification
    {
        #region Database Properties

        [Required]
        [ForeignKey("FK_DisplayEndpointNotification_DisplayEndpoint_DisplayEndpointID")]
        [Column("DisplayEndpointID")]
        public long DisplayEndpointID { get; set; }                     // FOREIGN Key to -> DisplayEndpoint:DisplayEndpointID

        [Required]
        [ForeignKey("FK_DisplayEndpointNotification_Notification_NotificationID")]
        [Column("NotificationID")]
        public long NotificationID { get; set; }                        // UNIQUE Key; FOREIGN Key to -> Notification:NotificationID

        #endregion

        #region Navigation Properties

        public DisplayEndpoint DisplayEndpoint { get; set; }
        public Notification Notification { get; set; }

        #endregion
    }

    [Table("Coupon")]
    public class Coupon
    {
        #region Database Properties

        [Key]
        public long CouponID { get; set; }                              // PRIMARY Key

        [Required]
        [ForeignKey("FK_Coupon_ClientSpot_ClientSpotID")]
        [Column("ClientSpotID")]
        public long ClientSpotID { get; set; }                          // FOREIGN Key to -> ClientSpot:ClientSpotID

        [ForeignKey("FK_Coupon_Notification_NotificationID")]
        [Column("NotificationID")]
        public long? NotificationID { get; set; }                       // FOREIGN Key to -> Notification:NotificationID

        [Required]
        [Column("Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Required]
        [Column("CouponCode")]
        [StringLength(100, MinimumLength = 1)]
        public string CouponCode { get; set; }

        [Required]
        [Column("Description")]
        [StringLength(50, MinimumLength = 3)]
        public string Description { get; set; }

        [Required]
        [DataType(DataType.Currency)]
        [Column("DiscountCents")]
        [Range(0, 10000000)]
        public double DiscountCents { get; set; }

        #endregion

        #region Navigation Properties

        public ClientSpot ClientSpot { get; set; }
        public Notification Notification { get; set; }

        #endregion
    }

    [Table("User")]
    public class User
    {
        #region Database Properties

        [Key]
        public long UserID { get; set; }                                // PRIMARY Key

        [Required]
        [EnumDataType(typeof(UserType))]
        [Column("Type")]
        [Range(0, 2)]
        public UserType Type { get; set; }

        [Column("Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Column("Email")]
        [StringLength(50, MinimumLength = 3)]
        public string Email { get; set; }                               // UNIQUE Key

        #endregion
    }

    #endregion

    #region Transactional Data Models

    [Table("ClientUser")]
    public class ClientUser
    {
        #region Database Properties

        [Required]
        [ForeignKey("FK_ClientUser_Client_ClientID")]
        [Column("ClientID")]
        public long ClientID { get; set; }                              // FOREIGN Key to -> Client:ClientID

        [Required]
        [ForeignKey("FK_ClientUser_User_UserID")]
        [Column("UserID")]
        public long UserID { get; set; }                                // FOREIGN Key to -> User:UserID

        [Required]
        [Column("VisitedAt")]
        public DateTime VisitedAt { get; set; }

        #endregion

        #region Navigation Properties

        public Client Client { get; set; }
        public User User { get; set; }

        #endregion

        #region Initialize

        public ClientUser()
        {
            // Set default values
            VisitedAt = DateTime.UtcNow;
        }

        #endregion
    }

    [Table("UserCoupon")]
    public class UserCoupon
    {
        #region Database Properties

        [Required]
        [ForeignKey("FK_UserCoupon_User_UserID")]
        [Column("UserID")]
        public long UserID { get; set; }                                // FOREIGN Key to -> User:UserID

        [Required]
        [ForeignKey("FK_UserCoupon_Coupon_CouponID")]
        [Column("CouponID")]
        public long CouponID { get; set; }                              // FOREIGN Key to -> Coupon:CouponID

        [Required]
        [Column("VisitedAt")]
        public DateTime ReceivedAt { get; set; }

        [Column("CouponRedempted")]
        public bool CouponRedempted { get; set; }

        #endregion

        #region Navigation Properties

        public User User { get; set; }
        public Coupon Coupon { get; set; }

        #endregion

        #region Initialize

        public UserCoupon()
        {
            // Set default values
            ReceivedAt = DateTime.UtcNow;
            CouponRedempted = false;
        }

        #endregion
    }

    #endregion

    #region Transient Data Models

    [DynamoDBTable("DisplayConcurrentList")]
    public class DisplayConcurrentList
    {
        #region Database Properties

        [DynamoDBHashKey]
        public Guid ID { get; set; }                                    // PRIMARY Key

        [DynamoDBProperty("DisplaySessions")]
        public List<DisplaySession> DisplaySessions { get; set; }

        [DynamoDBProperty("LastFlushedAt")]
        public DateTime? LastFlushedAt { get; set; }

        #endregion

        #region Initialize

        public DisplayConcurrentList()
        {
            // Set default values
            ID = Guid.NewGuid();
            DisplaySessions = new List<DisplaySession>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a DisplaySession or creates new if not exists
        /// </summary>
        /// <param name="displayEndpointID">DisplayEndpoint ID</param>
        /// <returns></returns>
        public DisplaySession ObtainDisplaySession(long displayEndpointID)
        {
            DisplaySession displaySession = null;

            // Try to get the DisplayEndpoint's respective DisplaySession from DisplayConcurrentList, create new if not exists.
            displaySession = DisplaySessions?
                .Where(dS => dS.DisplayEndpointID == displayEndpointID)
                .FirstOrDefault();
            if (displaySession == null)
            {
                // Create a new DisplaySession for the DisplayEndpoint and add to DisplaySessions
                DisplaySessions.Add(displaySession = new DisplaySession(displayEndpointID));
            }

            return displaySession;
        }

        #endregion
    }

    public class DisplaySession
    {
        #region Database Properties

        public long DisplayEndpointID { get; set; }                     // UNIQUE Key; FOREIGN Key to -> DisplayEndpoint:DisplayEndpointID
        public bool IsUserExists { get; set; }
        public long? BufferedShowNotificationID { get; set; }           // FOREIGN Key to -> Notification:NotificationID
        public DateTime? CurrentShowNotificationExpireAt { get; set; }
        public long? DisplayTouchedNotificationID { get; set; }         // FOREIGN Key to -> Notification:NotificationID
        public DateTime? DisplayTouchedAt { get; set; }
        public long? LocationDeviceID { get; set; }					    // FOREIGN Key to -> LocationDevice:LocationDeviceID
        public DateTime? LocationDeviceRegisteredAt { get; set; }

        #endregion

        #region Initialize

        public DisplaySession() { }

        public DisplaySession(long displayEndpointID)
        {
            DisplayEndpointID = displayEndpointID;
        }

        #endregion
    }

    [DynamoDBTable("ZoneConcurrentList")]
    public class ZoneConcurrentList
    {
        #region Database Properties

        [DynamoDBHashKey]
        public Guid ID { get; set; }                                    // PRIMARY Key

        [DynamoDBProperty("ZoneSessions")]
        public List<ZoneSession> ZoneSessions { get; set; }

        #endregion

        #region Initialize

        public ZoneConcurrentList()
        {
            // Set default values
            ID = Guid.NewGuid();
            ZoneSessions = new List<ZoneSession>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets a ZoneSession or creates new if not exists
        /// </summary>
        /// <param name="zoneID">Zone ID</param>
        /// <returns></returns>
        public ZoneSession ObtainZoneSession(long zoneID)
        {
            ZoneSession zoneSession = null;

            // Try to get the Zone's respective ZoneSession from ZoneConcurrentList, create new if not exists.
            zoneSession = ZoneSessions?
                .Where(zS => zS.ZoneID == zoneID)
                .FirstOrDefault();
            if (zoneSession == null)
            {
                // Create a new ZoneSession for the Zone and add to ZoneSessions
                ZoneSessions.Add(zoneSession = new ZoneSession(zoneID));
            }

            return zoneSession;
        }

        #endregion
    }

    public class ZoneSession
    {
        #region Database Properties

        public long ZoneID { get; set; }                                // UNIQUE Key; FOREIGN Key to -> Zone:ZoneID
        public UserConcurrentList UserConcurrentList { get; set; }

        #endregion

        #region Initialize

        public ZoneSession()
        {
            // Set default values
            UserConcurrentList = new UserConcurrentList();
        }

        public ZoneSession(long zoneID)
        {
            ZoneID = zoneID;
            // Set default values
            UserConcurrentList = new UserConcurrentList();
        }

        #endregion
    }

    public class UserConcurrentList
    {
        #region Database Properties

        public List<UserSession> UserSessions { get; set; }
        public DateTime? LastFlushedAt { get; set; }

        #endregion

        #region Initialize

        public UserConcurrentList()
        {
            // Set default values
            UserSessions = new List<UserSession>();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets an UserSession or creates new if not exists
        /// </summary>
        /// <param name="userID">User ID</param>
        /// <returns></returns>
        public UserSession ObtainUserSession(long userID)
        {
            UserSession userSession = null;

            // Try to get the User's respective UserSession from UserConcurrentList, create new if not exists.
            userSession = UserSessions?
                .Where(uS => uS.UserID == userID)
                .FirstOrDefault();
            if (userSession == null)
            {
                // Create new UserSession for the User and add to UserSessions
                UserSessions.Add(userSession = new UserSession(userID));
            }

            return userSession;
        }

        #endregion
    }

    public class UserSession
    {
        #region Database Properties

        public long UserID { get; set; }                                // UNIQUE Key; FOREIGN Key to -> User:UserID
        public DateTime? EnteredIntoZoneAt { get; set; }
        public DateTime? LastSeenInZoneAt { get; set; }
        public List<long> ReceivedCouponIDs { get; set; }               // For each item FOREIGN Key to -> Coupon:CouponID

        #endregion

        #region Initialize

        public UserSession()
        {
            // Set default values
            ReceivedCouponIDs = new List<long>();
        }

        public UserSession(long userID)
        {
            UserID = userID;
            // Set default values
            ReceivedCouponIDs = new List<long>();
        }

        #endregion
    }

    #endregion

    #region Database Contexts

    /// <summary>
    /// Relational Database Context : PostgreSQL
    /// </summary>
    public class RdsDbContext : DbContext
    {
        public RdsDbContext(DbContextOptions<RdsDbContext> options) : base(options)
        {
            // Automatically creates the empty database if not exists
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configuration Data Tables
            modelBuilder.Entity<Client>();
            modelBuilder.Entity<ClientSpot>();
            modelBuilder.Entity<Zone>();
            modelBuilder.Entity<LocationDevice>().HasIndex(lD => lD.DeviceID).IsUnique();
            modelBuilder.Entity<DisplayEndpoint>();
            modelBuilder.Entity<Notification>();
            modelBuilder.Entity<LocationDeviceNotification>().HasKey(lDN => new { lDN.LocationDeviceID, lDN.NotificationID });
            modelBuilder.Entity<LocationDeviceNotification>().HasIndex(lDN => lDN.LocationDeviceID).IsUnique();
            modelBuilder.Entity<LocationDeviceNotification>().HasIndex(lDN => lDN.NotificationID).IsUnique();
            modelBuilder.Entity<DisplayEndpointNotification>().HasKey(dEN => new { dEN.DisplayEndpointID, dEN.NotificationID });
            modelBuilder.Entity<DisplayEndpointNotification>().HasIndex(dEN => dEN.NotificationID).IsUnique();
            modelBuilder.Entity<Coupon>();
            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();

            // Transactional Data Tables
            modelBuilder.Entity<ClientUser>().HasKey(e => new { e.ClientID, e.UserID, e.VisitedAt });
            modelBuilder.Entity<UserCoupon>().HasKey(e => new { e.UserID, e.CouponID, e.ReceivedAt });
        }
    }

    /// <summary>
    /// NoSQL Database Context : AWS DynamoDB
    /// </summary>
    public class NosqlDbContext : DynamoDBContext
    {
        public NosqlDbContext(AmazonDynamoDBClient client, DynamoDBContextConfig config) : base(client, config) { }
    }

    #endregion

    #region Data Architecture

    /// <summary>
    /// Configuration Data
    /// </summary>
    public class ConfigurationData : RdsDbContext
    {
        #region Data Members

        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientSpot> ClientSpots { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<LocationDevice> LocationDevices { get; set; }
        public DbSet<DisplayEndpoint> DisplayEndpoints { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<LocationDeviceNotification> LocationDeviceNotifications { get; set; }
        public DbSet<DisplayEndpointNotification> DisplayEndpointNotifications { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<User> Users { get; set; }

        #endregion

        #region Initialize

        public ConfigurationData(DbContextOptions<RdsDbContext> options) : base(options) { }

        #endregion
    }

    /// <summary>
    /// Transactional Data
    /// </summary>
    public class TransactionalData : RdsDbContext
    {
        #region Data Members

        public DbSet<ClientUser> ClientUsers { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }

        #endregion

        #region Initialize

        public TransactionalData(DbContextOptions<RdsDbContext> options) : base(options) { }

        #endregion
    }

    /// <summary>
    /// Transient Data
    /// </summary>
    public class TransientData : NosqlDbContext
    {
        #region Data Members

        private DisplayConcurrentList _displayConcurrentList { get; set; }
        private ZoneConcurrentList _zoneConcurrentList { get; set; }

        #endregion

        #region Initialize

        public TransientData(AmazonDynamoDBClient client, DynamoDBContextConfig config) : base(client, config) { }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets freshly scanned DisplayConcurrentList or creates new if not exists
        /// </summary>
        /// <returns></returns>
        public async Task<DisplayConcurrentList> ObtainDisplayConcurrentListAsync()
        {
            return _displayConcurrentList = (await ScanAsync<DisplayConcurrentList>(null)?.GetNextSetAsync())?.FirstOrDefault() ?? new DisplayConcurrentList();
        }

        /// <summary>
        /// Gets freshly scanned ZoneConcurrentList or creates new if not exists
        /// </summary>
        /// <returns></returns>
        public async Task<ZoneConcurrentList> ObtainZoneConcurrentListAsync()
        {
            return _zoneConcurrentList = (await ScanAsync<ZoneConcurrentList>(null)?.GetNextSetAsync())?.FirstOrDefault() ?? new ZoneConcurrentList();
        }

        /// <summary>
        /// Saves DisplayConcurrentList
        /// </summary>
        /// <returns></returns>
        public async Task SaveDisplayConcurrentListAsync()
        {
            if (_displayConcurrentList != null) { await SaveAsync(_displayConcurrentList); }
        }

        /// <summary>
        /// Saves ZoneConcurrentList
        /// </summary>
        /// <returns></returns>
        public async Task SaveZoneConcurrentListAsync()
        {
            if (_zoneConcurrentList != null) { await SaveAsync(_zoneConcurrentList); }
        }

        #endregion
    }

#endregion
}
