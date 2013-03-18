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

            _myCue = soundCue;
            _myCue.Apply3D(_listner, _emitter);

            _entityAttachedTo = entityToAttachTo;
            _myCue.Play();
        }

        public void update()
        {
            _emitter.Position = _entityAttachedTo.getMatrix().Translation;
            _myCue.Apply3D(_listner, _emitter);
        }

        Cue _myCue;

        //SoundEffectInstance _soundInstance;
        AudioEmitter _emitter;
        AudioListener _listner;
        Entity _entityAttachedTo;
    }
}
