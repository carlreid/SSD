﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

namespace SSD
{
    class SoundAttacher
    {

        public SoundAttacher(Cue soundCue, AudioListener listner, Entity entityToAttachTo, bool isLoop = false)
        {
            _emitter = new AudioEmitter();
            _listner = listner;
            _emitter.Position = entityToAttachTo.getMatrix().Translation;
            _isPlaying = true;
            _isLoop = isLoop;
            _timeRemaining = null;

            _myCue = soundCue;
            _myCue.Apply3D(_listner, _emitter);

            _entityAttachedTo = entityToAttachTo;
            _myCue.Play();
        }

        public SoundAttacher(Cue soundCue, bool isLoop = false)
        {
            _emitter = new AudioEmitter();
            _listner = null;
            _isPlaying = true;

            _myCue = soundCue;

            _entityAttachedTo = null;
            _myCue.Play();
        }

        public SoundAttacher(Cue soundCue, int timeInMilis)
        {
            _emitter = new AudioEmitter();
            _listner = null;
            _isPlaying = true;
            _timeRemaining = timeInMilis;

            _myCue = soundCue;

            _entityAttachedTo = null;
            _myCue.Play();
        }

        public void update(GameTime gameTime)
        {
            if (_myCue.IsPaused)
            {
                return;
            }

            if (_timeRemaining != null)
            {
                _timeRemaining -= gameTime.ElapsedGameTime.Milliseconds;
                if (_timeRemaining <= 0)
                {
                    _isPlaying = false;
                    _myCue.Stop(AudioStopOptions.Immediate);
                }
            }

            //If the sound has stopped, dispose of the sound
            if (!_myCue.IsPlaying)
            {
                _myCue.Dispose();
                _isPlaying = false;
            }

            if (!_isPlaying || _entityAttachedTo == null)
            {
                return;
            }

            if (_entityAttachedTo.getAlive() == false && _isLoop)
            {
                _myCue.Stop(AudioStopOptions.Immediate);
                _isPlaying = false;
                _myCue.Dispose();
                return;
            }
            _emitter.Position = _entityAttachedTo.getMatrix().Translation;
            _myCue.Apply3D(_listner, _emitter);
        }

        public bool isPlaying()
        {
            return _isPlaying;
        }

        public void forceStop()
        {
            _isPlaying = false;
            _myCue.Stop(AudioStopOptions.Immediate);
            _myCue.Dispose();
        }

        public void pause()
        {
            if (!_myCue.IsDisposed && !_myCue.IsStopped)
            {
                _myCue.Pause();
            }
        }

        public void resume()
        {
            if (!_myCue.IsDisposed && !_myCue.IsStopped)
            {
                _myCue.Resume();
            }
        }

        Cue _myCue;

        //SoundEffectInstance _soundInstance;
        AudioEmitter _emitter;
        AudioListener _listner;
        Entity _entityAttachedTo;
        bool _isPlaying;
        bool _isLoop;
        int? _timeRemaining;
    }
}
