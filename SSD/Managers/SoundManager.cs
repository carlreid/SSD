using System.Collections.Generic;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;

namespace SSD
{
    public enum LoadedSounds { FIRE_ACTIVATED, ICE_ACTIVATED, ICE_BULLET_FIRED, FIRE_BULLET_FIRED, ICE_BULLET_DESTROY, FIRE_BULLET_DESTROY,
                               ROCK_ENEMY_MOVE, MINE_ENEMY_MOVE, ROCK_ENEMY_DESTROY, MINE_ENEMY_DESTROY,
                               SHIP_BOOST, SHIP_ENGINE_DRONE, SHIP_DEATH, SHIP_BOMB};
    class SoundManager
    {
        public SoundManager(ContentManager contentManager, PlayerEntity player)
        {
            _contentManager = contentManager;
            _soundAttatchments = new List<SoundAttacher>();
            //_soundEffects = new List<SoundEffect>();

            //_soundEffects.Add(_contentManager.Load<SoundEffect>("missile_sound"));

            _listner = new AudioListener();
            _player = player;
            _listner.Position = _player.getMatrix().Translation;

            _audioEngine = new AudioEngine("Content/XACTProject.xgs");
            _bulletWaveBank = new WaveBank(_audioEngine, "Content/BulletWaveBank.xwb");
            _bulletSoundBank = new SoundBank(_audioEngine, "Content/BulletSoundBank.xsb");
            _announcerWaveBank = new WaveBank(_audioEngine, "Content/AnnouncerWaveBank.xwb");
            _announcerSoundBank = new SoundBank(_audioEngine, "Content/AnnouncerSoundBank.xsb");
            _entityWaveBank = new WaveBank(_audioEngine, "Content/EntityWaveBank.xwb");
            _entitySoundBank = new SoundBank(_audioEngine, "Content/EntitySoundBank.xsb");

            _mainMusic = contentManager.Load<Song>("Music/game_music");
            MediaPlayer.Volume = 0.05f;
            //MediaPlayer.Play(_mainMusic);
        }

        public void addAttatchment(LoadedSounds soundEnum, Entity entityToAttachTo)
        {
            if (soundEnum == LoadedSounds.FIRE_ACTIVATED)
            {
                _soundAttatchments.Add(new SoundAttacher(_announcerSoundBank.GetCue("FireActivated")));
            }
            else if (soundEnum == LoadedSounds.ICE_ACTIVATED)
            {
                _soundAttatchments.Add(new SoundAttacher(_announcerSoundBank.GetCue("IceActivated")));
            }
            else if (soundEnum == LoadedSounds.ICE_BULLET_FIRED)
            {
                _soundAttatchments.Add(new SoundAttacher(_bulletSoundBank.GetCue("IceBulletFired"), _listner, entityToAttachTo));
            }
            else if (soundEnum == LoadedSounds.FIRE_BULLET_FIRED)
            {
                _soundAttatchments.Add(new SoundAttacher(_bulletSoundBank.GetCue("FireBulletFired"), _listner, entityToAttachTo));
            }
            else if (soundEnum == LoadedSounds.FIRE_BULLET_DESTROY)
            {
                _soundAttatchments.Add(new SoundAttacher(_bulletSoundBank.GetCue("FireBulletDestroy"), _listner, entityToAttachTo));
            }
            else if (soundEnum == LoadedSounds.ICE_BULLET_DESTROY)
            {
                _soundAttatchments.Add(new SoundAttacher(_bulletSoundBank.GetCue("IceBulletDestroy"), _listner, entityToAttachTo));
            }
            else if (soundEnum == LoadedSounds.ROCK_ENEMY_MOVE)
            {
                _soundAttatchments.Add(new SoundAttacher(_entitySoundBank.GetCue("RockMoving"), _listner, entityToAttachTo, true));
            }
            else if (soundEnum == LoadedSounds.MINE_ENEMY_MOVE)
            {
                _soundAttatchments.Add(new SoundAttacher(_entitySoundBank.GetCue("MineMoving"), _listner, entityToAttachTo, true));
            }
            else if (soundEnum == LoadedSounds.MINE_ENEMY_DESTROY)
            {
                _soundAttatchments.Add(new SoundAttacher(_entitySoundBank.GetCue("MineExplode"), _listner, entityToAttachTo));
            }
            else if (soundEnum == LoadedSounds.MINE_ENEMY_DESTROY)
            {
                _soundAttatchments.Add(new SoundAttacher(_entitySoundBank.GetCue("RockExplode"), _listner, entityToAttachTo));
            }
            else if (soundEnum == LoadedSounds.SHIP_BOOST)
            {
                _soundAttatchments.Add(new SoundAttacher(_entitySoundBank.GetCue("ShipBoost")));
            }
            else if (soundEnum == LoadedSounds.SHIP_DEATH)
            {
                _soundAttatchments.Add(new SoundAttacher(_entitySoundBank.GetCue("ShipDeath")));
            }
            else if (soundEnum == LoadedSounds.SHIP_BOMB)
            {
                _soundAttatchments.Add(new SoundAttacher(_entitySoundBank.GetCue("ShipBomb")));
            }

            
        }

        public void update()
        {
            _audioEngine.Update();

            //Update the listner position to be that of the player
            float backupYaw = _player.getYaw();
            _player.setYaw(90);
            Matrix playerMatrix = _player.getMatrix();
            _listner.Forward = playerMatrix.Forward;
            _listner.Up = playerMatrix.Up;
            _listner.Position = playerMatrix.Translation;
            _player.setYaw(MathHelper.ToDegrees(backupYaw));

            //Remove any sounds that are marked as not being played.
            _soundAttatchments.RemoveAll(delegate(SoundAttacher e)
                {
                    return !e.isPlaying();
                }
            );  

            //Run update on all the sounds
            foreach(SoundAttacher attatchedSound in _soundAttatchments){
                attatchedSound.update();
            }
        }

        public void playDeathSound(Entity entity)
        {
            if (entity is IceBullet)
            {
                addAttatchment(LoadedSounds.ICE_BULLET_DESTROY, entity);
            }
            else if (entity is FireBullet)
            {
                addAttatchment(LoadedSounds.FIRE_BULLET_DESTROY, entity);
            }
            else if (entity is EnemyRock)
            {
                addAttatchment(LoadedSounds.ROCK_ENEMY_DESTROY, entity);
            }
            else if (entity is EnemyMine)
            {
                addAttatchment(LoadedSounds.MINE_ENEMY_DESTROY, entity);
            }
            else if(entity is PlayerEntity)
            {
                addAttatchment(LoadedSounds.SHIP_DEATH, entity);
            }

        }

        AudioEngine _audioEngine;
   
        WaveBank _bulletWaveBank;
        SoundBank _bulletSoundBank;
        WaveBank _announcerWaveBank;
        SoundBank _announcerSoundBank;
        WaveBank _entityWaveBank;
        SoundBank _entitySoundBank;

        ContentManager _contentManager;
        List<SoundAttacher> _soundAttatchments;
        //List<SoundEffect> _soundEffects;
        AudioListener _listner;
        PlayerEntity _player;
        Song _mainMusic;
    }
}
