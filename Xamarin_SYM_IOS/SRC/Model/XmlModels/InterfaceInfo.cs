﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這段程式碼是由工具產生的。
//     執行階段版本:4.0.30319.42000
//
//     對這個檔案所做的變更可能會造成錯誤的行為，而且如果重新產生程式碼，
//     變更將會遺失。
// </auto-generated>
//------------------------------------------------------------------------------

using System.Xml.Serialization;

// 
// 此原始程式碼由 xsd 版本=4.6.1590.0 自動產生。
// 


/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class VehicleInfo {
    
    private ECU[] eCUsField;
    
    private string vinField;
    
    private string vehicleNameField;
    
    private int vehicleIdField;
    
    private string pictureField;
    
    private string modCodeField;
    
    private long diagTimestampField;
    
    private string brandField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("ECU", IsNullable=false)]
    public ECU[] ECUs {
        get {
            return this.eCUsField;
        }
        set {
            this.eCUsField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string vin {
        get {
            return this.vinField;
        }
        set {
            this.vinField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string vehicleName {
        get {
            return this.vehicleNameField;
        }
        set {
            this.vehicleNameField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public int vehicleId {
        get {
            return this.vehicleIdField;
        }
        set {
            this.vehicleIdField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string picture {
        get {
            return this.pictureField;
        }
        set {
            this.pictureField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string modCode {
        get {
            return this.modCodeField;
        }
        set {
            this.modCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public long diagTimestamp {
        get {
            return this.diagTimestampField;
        }
        set {
            this.diagTimestampField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string brand {
        get {
            return this.brandField;
        }
        set {
            this.brandField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class ECU {
    
    private LvData[] lvDatasField;
    
    private DTCode[] dTCodesField;
    
    private FreezeData[] freezeDatasField;
    
    private string idField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("LvData", IsNullable=false)]
    public LvData[] LvDatas {
        get {
            return this.lvDatasField;
        }
        set {
            this.lvDatasField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("DTCode", IsNullable=false)]
    public DTCode[] DTCodes {
        get {
            return this.dTCodesField;
        }
        set {
            this.dTCodesField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlArrayItemAttribute("FreezeData", IsNullable=false)]
    public FreezeData[] FreezeDatas {
        get {
            return this.freezeDatasField;
        }
        set {
            this.freezeDatasField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class LvData {
    
    private short idField;
    
    private float valueField;
    
    private uint displayOrderField;
    
    private long diagTimestampField;
    
    public LvData() {
        this.displayOrderField = ((uint)(65535));
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public short id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public float value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    [System.ComponentModel.DefaultValueAttribute(typeof(uint), "65535")]
    public uint displayOrder {
        get {
            return this.displayOrderField;
        }
        set {
            this.displayOrderField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public long diagTimestamp {
        get {
            return this.diagTimestampField;
        }
        set {
            this.diagTimestampField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class DTCode {
    
    private string hexCodeField;
    
    private long diagTimestampField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string hexCode {
        get {
            return this.hexCodeField;
        }
        set {
            this.hexCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public long diagTimestamp {
        get {
            return this.diagTimestampField;
        }
        set {
            this.diagTimestampField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class FreezeData {
    
    private short idField;
    
    private float valueField;
    
    private string hexCodeField;
    
    private long diagTimestampField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public short id {
        get {
            return this.idField;
        }
        set {
            this.idField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public float value {
        get {
            return this.valueField;
        }
        set {
            this.valueField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string hexCode {
        get {
            return this.hexCodeField;
        }
        set {
            this.hexCodeField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public long diagTimestamp {
        get {
            return this.diagTimestampField;
        }
        set {
            this.diagTimestampField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class LvDatas {
    
    private LvData[] lvDataField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("LvData")]
    public LvData[] LvData {
        get {
            return this.lvDataField;
        }
        set {
            this.lvDataField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class InterfaceInfo {
    
    private VehicleInfo vehicleInfoField;
    
    private string softwareVersionField;
    
    private string serialNumberField;
    
    private string firmwareVersionField;
    
    private string databaseVersionField;
    
    /// <remarks/>
    public VehicleInfo VehicleInfo {
        get {
            return this.vehicleInfoField;
        }
        set {
            this.vehicleInfoField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string softwareVersion {
        get {
            return this.softwareVersionField;
        }
        set {
            this.softwareVersionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string serialNumber {
        get {
            return this.serialNumberField;
        }
        set {
            this.serialNumberField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string firmwareVersion {
        get {
            return this.firmwareVersionField;
        }
        set {
            this.firmwareVersionField = value;
        }
    }
    
    /// <remarks/>
    [System.Xml.Serialization.XmlAttributeAttribute()]
    public string databaseVersion {
        get {
            return this.databaseVersionField;
        }
        set {
            this.databaseVersionField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class FreezeDatas {
    
    private FreezeData[] freezeDataField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("FreezeData")]
    public FreezeData[] FreezeData {
        get {
            return this.freezeDataField;
        }
        set {
            this.freezeDataField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class ECUs {
    
    private ECU[] eCUField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("ECU")]
    public ECU[] ECU {
        get {
            return this.eCUField;
        }
        set {
            this.eCUField = value;
        }
    }
}

/// <remarks/>
[System.CodeDom.Compiler.GeneratedCodeAttribute("xsd", "4.6.1590.0")]
[System.SerializableAttribute()]
[System.Diagnostics.DebuggerStepThroughAttribute()]
[System.ComponentModel.DesignerCategoryAttribute("code")]
[System.Xml.Serialization.XmlTypeAttribute(AnonymousType=true)]
[System.Xml.Serialization.XmlRootAttribute(Namespace="", IsNullable=false)]
public partial class DTCodes {
    
    private DTCode[] dTCodeField;
    
    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("DTCode")]
    public DTCode[] DTCode {
        get {
            return this.dTCodeField;
        }
        set {
            this.dTCodeField = value;
        }
    }
}
