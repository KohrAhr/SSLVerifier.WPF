﻿using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Serialization;
using SSLVerifier.API.MainLogic;
using SSLVerifier.API.ViewModels;
using SSLVerifier.Properties;

namespace SSLVerifier.API.ModelObjects {
    [XmlType(AnonymousType = true)]
    public class ServerObject : ViewModelBase, IDisposable {
        X509Certificate2 cert;
        String name;
        Int32 port, progress;
        ServerStatusEnum status;

        public ServerObject() {
            status = ServerStatusEnum.Unknown;
            TempChain = new X509Chain(Settings.Default.AllowUserTrust);
            SAN = new ObservableCollection<String>();
            Log = new StringBuilderWrapper();
            Tree = new ObservableCollection<TreeNode<ChainElement>>();
            Proxy = new ProxyObject();
        }

        [XmlElement]
        public String ServerAddress {
            get => name;
            set {
                name = value;
                OnPropertyChanged(nameof(ServerAddress));
            }
        }
        [XmlElement]
        public Int32 Port {
            get => port;
            set {
                port = value;
                OnPropertyChanged(nameof(Port));
            }
        }
        public ProxyObject Proxy { get; set; }
        [XmlIgnore]
        public ServerStatusEnum ItemStatus {
            get => status;
            set {
                status = value;
                OnPropertyChanged(nameof(ItemStatus));
            }
        }
        [XmlIgnore]
        public Int32 ItemProgress {
            get => progress;
            set {
                progress = value;
                OnPropertyChanged(nameof(ItemProgress));
            }
        }
        [XmlIgnore]
        public StringBuilderWrapper Log { get; set; }
        [XmlIgnore]
        public String ValidFrom => Certificate?.NotBefore.ToShortDateString();

        [XmlIgnore]
        public String ValidTo => Certificate?.NotAfter.ToShortDateString();

        [XmlIgnore]
        public Int32 DaysLeft => Certificate == null ? 0 : (Certificate.NotAfter - DateTime.Now).Days;

        [XmlIgnore]
        public Boolean ShouldScan { get; set; }
        [XmlIgnore]
        public ObservableCollection<String> SAN { get; set; }
        [XmlIgnore]
        public X509ChainStatusFlags2 ChainStatus { get; set; }
        [XmlIgnore]
        public X509Certificate2 Certificate {
            get => cert;
            set {
                cert = value;
                OnPropertyChanged(nameof(Certificate));
                OnPropertyChanged(nameof(ValidFrom));
                OnPropertyChanged(nameof(ValidTo));
                OnPropertyChanged(nameof(DaysLeft));
            }
        }
        [XmlIgnore]
        public ChainElement Chain { get; set; }
        [XmlIgnore]
        public HttpWebRequest TempRequest { get; set; }
        [XmlIgnore]
        public HttpWebResponse TempResponse { get; set; }
        [XmlIgnore]
        public X509Chain TempChain { get; set; }
        [XmlIgnore]
        public ObservableCollection<TreeNode<ChainElement>> Tree { get; set; }
        [XmlIgnore]
        public Boolean CanProcess { get; set; }

        public void Dispose() {
            TempChain.Reset();
            TempResponse?.Close();
        }

        public override String ToString() {
            return Certificate == null
                ? "\"" + ServerAddress + "\"," + "\"" + Port + "\",,,,,"
                : "\"" + ServerAddress + "\"," + "\"" + Port + "\"," + "\"" + Certificate.Subject + "\"," + "\"" +
                Certificate.NotBefore + "\"," + "\"" + Certificate.NotAfter + "\"," + "\"" + ItemStatus + "\"";
        }

        public override Boolean Equals(Object obj) {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            return obj.GetType() == GetType() && Equals((ServerObject)obj);
        }
        public override Int32 GetHashCode() {
            unchecked {
                return (name.GetHashCode() * 397) ^ port;
            }
        }
        protected Boolean Equals(ServerObject other) {
            return String.Equals(name, other.name) && port == other.port;
        }
    }
}
