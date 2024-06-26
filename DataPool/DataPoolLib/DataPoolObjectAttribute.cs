﻿namespace DataPoolLib
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public class DataPoolObjectAttribute : Attribute
    {
        public DataPoolObjectAttribute(byte majorVersion, byte minorVersion)
        {
            MajorVersion = majorVersion;
            MinorVersion = minorVersion;
        }

        public byte MajorVersion { get; }
        public byte MinorVersion { get; }
    }
}
