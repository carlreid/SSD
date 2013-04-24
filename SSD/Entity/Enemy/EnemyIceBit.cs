using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SSD
{
    class EnemyIceBit
    {

        public EnemyIceBit(Vector3 startingPosition, float radius, ModelContainer myModel)
        {
            _currentPosition = startingPosition;
            _lastPosition = _currentPosition;
            _myModel = myModel;
            _myBoundingSphere.Center = startingPosition;
            _myBoundingSphere.Radius = radius;
        }

        public void updatePosition(Vector3 newPosition){
            _lastPosition = _currentPosition;
            _currentPosition = newPosition;

            _myBoundingSphere.Center = _currentPosition;
            foreach (ModelMesh mesh in _myModel.getModel().Meshes)
            {
                _myBoundingSphere.Radius = mesh.BoundingSphere.Radius;
            }
        }

        public Vector3 getPosition()
        {
            return _currentPosition;
        }

        public Vector3 getLastPosition()
        {
            return _lastPosition;
        }

        public BoundingSphere getBoundingSphere()
        {
            return _myBoundingSphere;
        }

        Vector3 _currentPosition;
        Vector3 _lastPosition;
        BoundingSphere _myBoundingSphere;
        ModelContainer _myModel;
    }
}
