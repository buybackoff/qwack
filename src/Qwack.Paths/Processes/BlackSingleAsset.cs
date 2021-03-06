﻿using Qwack.Options.VolSurfaces;
using Qwack.Paths.Features;
using System;
using System.Collections.Generic;
using System.Text;
using Qwack.Core.Underlyings;
using System.Numerics;

namespace Qwack.Paths.Processes
{
    public class BlackSingleAsset : IPathProcess
    {
        private IATMVolSurface _surface;
        private DateTime _expiryDate;
        private DateTime _startDate;
        private int _numberOfSteps;
        private string _name;
        private int _factorIndex;
        private ITimeStepsFeature _timesteps;
        private Func<double, double> _forwardCurve;

        private double[] _drifts;
        private double[] _vols;


        public BlackSingleAsset(IATMVolSurface volSurface, DateTime startDate, DateTime expiryDate, int nTimeSteps, Func<double,double> forwardCurve, string name)
        {
            _surface = volSurface;
            _startDate = startDate;
            _expiryDate = expiryDate;
            _numberOfSteps = nTimeSteps;
            _name = name;
            _forwardCurve = forwardCurve;
        }

        public void Process(PathBlock block)
        {
        

            for (var path = 0; path < block.NumberOfPaths; path += Vector<double>.Count)
            {
                //This should be set to the spot price here
                var previousStep = new Vector<double>(_forwardCurve(0));
                var steps = block.GetStepsForFactor(path, _factorIndex);
                steps[0] = previousStep;
                for (var step = 1; step < block.NumberOfSteps; step++)
                {
                    var drift = _drifts[step] * _timesteps.TimeSteps[step] * previousStep;
                    var delta = _vols[step] * steps[step] * previousStep;

                    previousStep = (previousStep + drift + delta);
                    steps[step] = previousStep;
                }
            }
        }

        public void SetupFeatures(FeatureCollection pathProcessFeaturesCollection)
        {
            _drifts = new double[_numberOfSteps];
            _vols = new double[_numberOfSteps];


            var mappingFeature = pathProcessFeaturesCollection.GetFeature<IPathMappingFeature>();
            _factorIndex = mappingFeature.AddDimension(_name);

            _timesteps = pathProcessFeaturesCollection.GetFeature<ITimeStepsFeature>();
            var stepSize = (_expiryDate - _startDate).TotalDays / _numberOfSteps;
            for (var i = 0; i < _numberOfSteps - 1; i++)
            {
                _timesteps.AddDate(_startDate.AddDays(i * stepSize));
            }
            _timesteps.AddDate(_expiryDate);
            pathProcessFeaturesCollection.FinishSetup();

            //drifts and vols...
            var prevSpot = _forwardCurve(0);
            for (var t = 1; t < _drifts.Length; t++)
            {
                var spot = _forwardCurve(_timesteps.Times[t]);
                _drifts[t] = System.Math.Log(spot / prevSpot) / _timesteps.TimeSteps[t];
                _vols[t] = _surface.GetForwardATMVol(_timesteps.Times[t - 1], _timesteps.Times[t]);
                _vols[t] *= System.Math.Sqrt(_timesteps.TimeSteps[t]);
                prevSpot = spot;
            }
        }
    }
}
