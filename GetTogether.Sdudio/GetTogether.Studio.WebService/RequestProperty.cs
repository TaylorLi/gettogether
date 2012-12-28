using System;
using System.Collections.Generic;
using System.Text;

namespace GetTogether.Studio.WebService
{
    public class RequestProperty
    {
        private string _Address;

        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }
        private int _Timeout;

        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        private string _ContentType;

        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        private string _Method;

        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        private string[] _Headers;

        public string[] Headers
        {
            get { return _Headers; }
            set { _Headers = value; }
        }
        private string _UserAgent;

        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        private string _Referer;

        public string Referer
        {
            get { return _Referer; }
            set { _Referer = value; }
        }
        private string _Accept;

        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        private string _Expect;

        public string Expect
        {
            get { return _Expect; }
            set { _Expect = value; }
        }


        private bool _UseDefaultCredential;

        public bool UseDefaultCredential
        {
            get { return _UseDefaultCredential; }
            set { _UseDefaultCredential = value; }
        }
        private string _BasicAuthUserName;

        public string BasicAuthUserName
        {
            get { return _BasicAuthUserName; }
            set { _BasicAuthUserName = value; }
        }
        private string _BasicAuthPassword;

        public string BasicAuthPassword
        {
            get { return _BasicAuthPassword; }
            set { _BasicAuthPassword = value; }
        }
        private bool _AllowAutoRedirect;

        public bool AllowAutoRedirect
        {
            get { return _AllowAutoRedirect; }
            set { _AllowAutoRedirect = value; }
        }
        private bool _AllowWriteStreamBuffering;

        public bool AllowWriteStreamBuffering
        {
            get { return _AllowWriteStreamBuffering; }
            set { _AllowWriteStreamBuffering = value; }
        }
        private bool _KeepAlive;

        public bool KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        private bool _Pipelined;

        public bool Pipelined
        {
            get { return _Pipelined; }
            set { _Pipelined = value; }
        }
        private bool _PreAuthenticate;

        public bool PreAuthenticate
        {
            get { return _PreAuthenticate; }
            set { _PreAuthenticate = value; }
        }
        private bool _SendChunked;

        public bool SendChunked
        {
            get { return _SendChunked; }
            set { _SendChunked = value; }
        }
        private string _TransferEncoding;

        public string TransferEncoding
        {
            get { return _TransferEncoding; }
            set { _TransferEncoding = value; }
        }
        private string _MediaType;

        public string MediaType
        {
            get { return _MediaType; }
            set { _MediaType = value; }
        }

        public RequestProperty()
        {

        }
    }
}
