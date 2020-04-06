/*
 * The program is developed as a data collection tool in the fields of motion 
 * analysis and physical condition.The user of the software is motivated to 
 * complete exercises through the use of Games. This program is available as
 * a part of the open source project OpenFeasyo found at
 * https://github.com/openfeasyo/OpenFeasyo>.
 * 
 * Copyright (c) 2020 - Lubos Omelina
 * 
 * This program is free software: you can redistribute it and/or modify it 
 * under the terms of the GNU General Public License version 3 as published 
 * by the Free Software Foundation. The Software Source Code is submitted 
 * within i-DEPOT holding reference number: 122388.
 */
namespace OpenFeasyo.Platform.Data
{
    public class RegistryElements
    {
        public const string REGISTRY_ROOT_SECTION = @"HKEY_CURRENT_USER\SOFTWARE\ict4rehab";
        internal const string REGISTRY_SECTION = @"HKEY_CURRENT_USER\SOFTWARE\ict4rehab\PatientsFor";
        internal const string REGISTRY_THERAPIST_TAG = "Current";
        
        public const string REGISTRY_FORCE_KINECT_1 = "ForceKinect1";

        internal const string REGISTRY_SERVER = @"Server";
        internal const string REGISTRY_ALLOW_ALL_CERTIFICATES = @"AllowAllCertificates";
    }
}
