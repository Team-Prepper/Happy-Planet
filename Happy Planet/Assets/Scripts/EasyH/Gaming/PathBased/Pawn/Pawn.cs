using UnityEngine;

namespace EasyH.Gaming.PathBased
{

    public class Pawn : MonoBehaviour
    {

        [SerializeField] private PawnMoveBase _moveOnMap;
        [SerializeField] private MoveToBase _initialMoveOnMap;
        [SerializeField] protected Plate _nowPlate;

        public virtual Plate MovePredict(int amount)
        {
            Plate plate = _nowPlate;

            if (plate == null)
            {
                plate = BoardManager.Instance.Map.GetStartPlate();
            }

            return _moveOnMap.Predict(plate, this, amount);

        }

        public virtual void Move(int amount)
        {

            if (_nowPlate != null)
            {
                PawnMove(amount);
                return;
            }

            //_owner.LeavePawn(Id);
            _nowPlate = BoardManager.Instance.Map.GetStartPlate();

            _initialMoveOnMap.MoveTo(_nowPlate.transform.position, () =>
            {
                PawnMove(amount);
            });

        }

        private void PawnMove(int amount)
        {
            _moveOnMap.MoveTo(_nowPlate, this, amount, ArriveGoal, (value) =>
            {
                value.Arrive(this);
            });
        }

        public virtual void SetPlateAt(Plate value)
        {
            _nowPlate = value;
        }

        public void Dispose(Vector3 pos)
        {
            _initialMoveOnMap.MoveTo(pos, null);
        }

        public virtual void ArriveGoal()
        {

        }

        public virtual void Killed()
        {
            BackHome();

            //SFXManager.Instance.PlaySFX("Kill");
            //Instantiate(_catchEffect, transform.position + Vector3.up, Quaternion.identity);
        }

        public virtual void BackHome()
        {

        }

        public virtual int Value()
        {
            return 1;
        }

    }
}