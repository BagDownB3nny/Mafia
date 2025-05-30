using System;
using NAudio.Dsp;
using NAudio.Wave;
using UnityEngine;

namespace Dissonance.Audio.Playback
{
    /// <summary>
    /// Resamples to the output rate of the Unity audio system
    /// </summary>
    internal class Resampler
        : ISampleSource
    {
        private static readonly Log Log = Logs.Create(LogCategory.Playback, nameof(Resampler));

        private readonly ISampleSource _source;
        private readonly IRateProvider _rate;

        private volatile WaveFormat _outputFormat;
        private readonly WdlResampler _resampler;

        private bool _fixedRateEnabled = false;

        public Resampler(ISampleSource source, IRateProvider rate)
        {
            _source = source;
            _rate = rate;

            AudioSettings.OnAudioConfigurationChanged += OnAudioConfigurationChanged;

            _resampler = new WdlResampler();
            _resampler.SetMode(true, 2, false);
            _resampler.SetFilterParms();
            _resampler.SetFeedMode(false);
        }

        public WaveFormat WaveFormat => _outputFormat;

        public void Prepare(SessionContext context)
        {
            OnAudioConfigurationChanged(false);

            _source.Prepare(context);
        }

        public bool Read(ArraySegment<float> samples)
        {
            var inFormat = _source.WaveFormat;
            var outFormat = _outputFormat;

            // Configure the rate of the resampler based on the requested playback rate.
            // If rate adjustment is very small (<1%) play back at the base rate, this means
            // in the normal case (rate=1) the rate won't be changing every frame
            var outputRate = (double)outFormat.SampleRate;
            if (Mathf.Abs(_rate.PlaybackRate - 1) > 0.01f)
                outputRate = outFormat.SampleRate * (1 / _rate.PlaybackRate);

            // ReSharper disable once CompareOfFloatsByEqualityOperator (justification: we want exact comparison)
            if (outputRate != _resampler.OutputSampleRate)
            {
                Log.Trace("Changing resampler rate to {0}Hz", outputRate);
                _resampler.SetRates(inFormat.SampleRate, outputRate);
            }

            var channels = inFormat.Channels;

            // prepare buffers
            var samplesPerChannelRequested = samples.Count / channels;
            var samplesPerChannelRequired = _resampler.ResamplePrepare(samplesPerChannelRequested, channels, out var inBuffer, out var inBufferOffset);
            var sourceBuffer = new ArraySegment<float>(inBuffer, inBufferOffset, samplesPerChannelRequired * channels);

            // read source
            var complete = _source.Read(sourceBuffer);

            // resample
            Log.Trace("Resampling {0}Hz -> {1}Hz", inFormat.SampleRate, outFormat.SampleRate);
            _resampler.ResampleOut(samples.Array, samples.Offset, samplesPerChannelRequired, samplesPerChannelRequested, channels);

            return complete;
        }

        public void Reset()
        {
            _resampler?.Reset();

            _source.Reset();
        }

        private void OnAudioConfigurationChanged(bool deviceWasChanged)
        {
            if (_fixedRateEnabled)
                return;

#if NCRUNCH
            _outputFormat = new WaveFormat(44100, _source.WaveFormat.Channels);
#else
            _outputFormat = new WaveFormat(AudioSettings.outputSampleRate, _source.WaveFormat.Channels);
#endif
        }

        /// <summary>
        /// Override automatic output sample rate determination and set it to a fixed value
        /// </summary>
        /// <param name="rate"></param>
        public void SetOutputRate(int? rate)
        {
            if (rate.HasValue)
            {
                _fixedRateEnabled = true;
                if (_outputFormat == null || _outputFormat.SampleRate != rate.Value)
                    _outputFormat = new WaveFormat(rate.Value, _source.WaveFormat.Channels);
            }
            else
            {
                _fixedRateEnabled = false;
                OnAudioConfigurationChanged(false);
            }
        }
    }
}