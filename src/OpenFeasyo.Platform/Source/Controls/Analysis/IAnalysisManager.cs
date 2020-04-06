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
namespace OpenFeasyo.Platform.Controls.Analysis
{
    public interface IAnalysisManager
    {
        bool HasSkeletonAnalyzer(string fileName);

        bool HasAccelerometerAnalyzer(string fileName);

        bool HasBalanceBoardAnalyzer(string fileName);

        bool HasEmgSignalAnalyzer(string fileName);

        ISkeletonAnalyzer GetSkeletonAnalyzer(string fileName);

        IAccelerometerAnalyzer GetAccelerometerAnalyzer(string fileName);

        IBalanceBoardAnalyzer GetBalanceBoardAnalyzer(string fileName);

        IEmgSignalAnalyzer GetEmgSignalAnalyzer(string fileName);

        string GetAnalyzerModuleName(ISkeletonAnalyzer analyzer);

        string GetAnalyzerModuleName(IAccelerometerAnalyzer analyzer);

        string GetAnalyzerModuleName(IBalanceBoardAnalyzer analyzer);

        string GetAnalyzerModuleName(IEmgSignalAnalyzer analyzer);

    }
}
