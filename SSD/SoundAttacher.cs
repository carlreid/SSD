using Microsoft.Xna.Framework;
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

        public void update()
        {
            if (!_isPlaying)
            {
                return;
            }

            if (_entityAttachedTo == null)
            {
                return;
            }

            //if (_entityAttachedTo.getAlive() == false || _entityAttachedTo == null)
            //{
            //    _myCue.Stop(AudioStopOptions.Immediate);
            //    _isPlaying = false;
            //    _myCue.Dispose();
            //    return;
            //}
            _emitter.Position = _entityAttachedTo.getMatrix().Translation;
            _myCue.Apply3D(_listner, _emitter);
        }

        public bool isPlaying()
        {
            return _isPlaying;
        }

        Cue _myCue;

        //SoundEffectInstance _soundInstance;
        AudioEmitter _emitter;
        AudioListener _listner;
        Entity _entityAttachedTo;
        bool _isPlaying;
    }
}
