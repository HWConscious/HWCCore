using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        // Navigation Properties
        public List<ClientSpot> ClientSpots { get; set; }
    }

    [Table("ClientSpot")]
    public class ClientSpot
    {
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

        // Navigation Properties
        public Client Client { get; set; }
        public List<Zone> Zones { get; set; }
        public List<LocationDevice> LocationDevices { get; set; }
        public List<DisplayEndpoint> DisplayEndpoints { get; set; }
        public List<Notification> Notifications { get; set; }
        public List<Coupon> Coupons { get; set; }
    }

    [Table("Zone")]
    public class Zone
    {
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

        // Navigation Properties
        public ClientSpot ClientSpot { get; set; }
        public List<LocationDevice> LocationDevices { get; set; }
        public List<DisplayEndpoint> DisplayEndpoints { get; set; }
    }

    [Table("LocationDevice")]
    public class LocationDevice
    {
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

        // Navigation Properties
        public ClientSpot ClientSpot { get; set; }
        public Zone Zone { get; set; }
    }

    [Table("DisplayEndpoint")]
    public class DisplayEndpoint
    {
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

        // Navigation Properties
        public ClientSpot ClientSpot { get; set; }
        public Zone Zone { get; set; }
        public List<Notification> Notifications { get; set; }
    }

    [Table("Notification")]
    public class Notification
    {
        [Key]
        public long NotificationID { get; set; }                        // PRIMARY Key

        [Required]
        [ForeignKey("FK_Notification_ClientSpot_ClientSpotID")]
        [Column("ClientSpotID")]
        public long ClientSpotID { get; set; }                          // FOREIGN Key to -> ClientSpot:ClientSpotID

        [ForeignKey("FK_Notification_DisplayEndpoint_DisplayEndpointID")]
        [Column("DisplayEndpointID")]
        public long? DisplayEndpointID { get; set; }                    // FOREIGN Key to -> DisplayEndpoint:DisplayEndpointID

        [Required]
        [Column("Name")]
        [StringLength(50, MinimumLength = 3)]
        public string Name { get; set; }

        [Column("SortOrder")]
        [Range(0, 50)]
        public int SortOrder { get; set; }

        [Required]
        [Column("Timeout")]
        [Range(0, 3600)]
        public int Timeout { get; set; }

        [Required]
        [Column("Active")]
        public bool Active { get; set; }

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

        // Navigation Properties
        public ClientSpot ClientSpot { get; set; }
        public DisplayEndpoint DisplayEndpoint { get; set; }
        public List<Coupon> Coupons { get; set; }
    }

    [Table("Coupon")]
    public class Coupon
    {
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

        // Navigation Properties
        public ClientSpot ClientSpot { get; set; }
        public Notification Notification { get; set; }
    }

    [Table("User")]
    public class User
    {
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
    }

    #endregion

    #region Transactional Data Models

    [Table("ClientUser")]
    public class ClientUser
    {
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

        // Navigation Properties
        public Client Client { get; set; }
        public User User { get; set; }
    }

    [Table("UserCoupon")]
    public class UserCoupon
    {
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

        // Navigation Properties
        public User User { get; set; }
        public Coupon Coupon { get; set; }
    }

    #endregion

    #region Transient Data Models

    [DynamoDBTable("DisplayConcurrentList")]
    public class DisplayConcurrentList
    {
        [DynamoDBHashKey]
        public Guid ID { get; set; }                                    // PRIMARY Key

        [DynamoDBProperty("DisplaySessions")]
        public List<DisplaySession> DisplaySessions { get; set; }

        [DynamoDBProperty("LastFlushedAt")]
        public DateTime? LastFlushedAt { get; set; }
    }

    public class DisplaySession
    {
        public long DisplayEndpointID { get; set; }                     // UNIQUE Key; FOREIGN Key to -> DisplayEndpoint:DisplayEndpointID
        public bool IsUserExists { get; set; }
        public long? BufferedShowNotificationID { get; set; }           // FOREIGN Key to -> Notification:NotificationID
        public DateTime? CurrentShowNotificationExpireAt { get; set; }
        public long? DisplayTouchedNotificationID { get; set; }         // FOREIGN Key to -> Notification:NotificationID
        public DateTime? DisplayTouchedAt { get; set; }
    }

    [DynamoDBTable("ZoneConcurrentList")]
    public class ZoneConcurrentList
    {
        [DynamoDBHashKey]
        public Guid ID { get; set; }                                    // PRIMARY Key

        [DynamoDBProperty("ZoneSessions")]
        public List<ZoneSession> ZoneSessions { get; set; }
    }

    public class ZoneSession
    {
        public long ZoneID { get; set; }                                // UNIQUE Key; FOREIGN Key to -> Zone:ZoneID
        public UserConcurrentList UserConcurrentList { get; set; }
    }

    public class UserConcurrentList
    {
        public List<UserSession> UserSessions { get; set; }
        public DateTime? LastFlushedAt { get; set; }
    }

    public class UserSession
    {
        public long UserID { get; set; }                                // UNIQUE Key; FOREIGN Key to -> User:UserID
        public DateTime? EnteredIntoZoneAt { get; set; }
        public DateTime? LastSeenInZoneAt { get; set; }
        public List<long> ReceivedCouponIDs { get; set; }               // For each item FOREIGN Key to -> Coupon:CouponID
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
        public DbSet<Client> Clients { get; set; }
        public DbSet<ClientSpot> ClientSpots { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<LocationDevice> LocationDevices { get; set; }
        public DbSet<DisplayEndpoint> DisplayEndpoints { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<User> Users { get; set; }

        public ConfigurationData(DbContextOptions<RdsDbContext> options) : base(options) { }
    }

    /// <summary>
    /// Transactional Data
    /// </summary>
    public class TransactionalData : RdsDbContext
    {
        public DbSet<ClientUser> ClientUsers { get; set; }
        public DbSet<UserCoupon> UserCoupons { get; set; }

        public TransactionalData(DbContextOptions<RdsDbContext> options) : base(options) { }
    }

    /// <summary>
    /// Transient Data
    /// </summary>
    public class TransientData : NosqlDbContext
    {
        public TransientData(AmazonDynamoDBClient client, DynamoDBContextConfig config) : base(client, config) { }
    }

    #endregion
}
