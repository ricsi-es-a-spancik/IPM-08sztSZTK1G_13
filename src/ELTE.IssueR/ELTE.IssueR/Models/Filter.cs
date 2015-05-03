namespace ELTE.IssueR.Models
{
    using System;
    using System.IO;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Xml;

    public partial class Filter
    {
        #region table columns

        public int Id { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "A szűrő nevének megadása kötelező!")]
        [StringLength(15, ErrorMessage = "A szűrő nevének hossza maximum 15 karakter lehet!")]
        public string Name { get; set; }

        public int ProjectId { get; set; }

        public byte[] UserIds { get; set; }
        
        public byte[] Statuses { get; set; }

        public byte[] Types { get; set; }

        public Nullable<int> DeadlineInterval { get; set; }

        #endregion table columns

        #region virtual columns

        public virtual Project Project { get; set; }

        #endregion virtual columns

        #region serialization

        public string[] DeserializedUserIds
        {
            get { return Deserialize<string>(UserIds); }
            set { UserIds = Serialize(value); }
        }

        public IssueStatus[] DeserializedStatuses
        {
            get { return Deserialize<IssueStatus>(Statuses); }
            set { Statuses = Serialize(value); }
        }

        public IssueType[] DeserializedTypes
        {
            get { return Deserialize<IssueType>(Types); }
            set { Types = Serialize(value); }
        }

        private byte[] Serialize<T>(T[] obj)
        {
            if (obj == null)
            {
                return new byte[0];
            }
            else
            {
                MemoryStream stream = new MemoryStream();
                BinaryFormatter bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, obj);
                return stream.ToArray();
            }
        }

        private T[] Deserialize<T>(byte[] obj)
        {
            if (obj == null)
            {
                return new T[0];
            }
            else
            {
                MemoryStream stream = new MemoryStream(obj);
                BinaryFormatter bFormatter = new BinaryFormatter();
                return (T[])bFormatter.Deserialize(stream);
            }
        }

        #endregion serialization

        #region properties

        public IEnumerable<string> UserNames { get; set; }
        public IEnumerable<string> TypeTexts { get; set; }
        public IEnumerable<string> StatusTexts { get; set; }

        #endregion properties

        #region methods

        public bool IsValid()
        {
            return UserIds != null || Statuses != null || Types != null || DeadlineInterval != null;
        }

        #endregion methods
    }
}