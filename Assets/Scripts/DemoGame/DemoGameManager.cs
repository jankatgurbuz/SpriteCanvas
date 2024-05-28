using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DemoGame
{
    public class DemoGameManager : MonoBehaviour
    {
        [SerializeField] private int _rowLength;
        [SerializeField] private int _columnLength;

        private Grid _grid;
        private DemoGameBeads _beads;

        private BeadController[,] _beadControl;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
            _beads = GetComponent<DemoGameBeads>();
            Create();
        }

        private void Create()
        {
            _beadControl = new BeadController[_rowLength, _columnLength];
            for (int i = 0; i < _rowLength; i++)
            {
                for (int j = 0; j < _columnLength; j++)
                {
                    var (obj,color) = _beads.GetRandomObject();
                    obj = Instantiate(obj, transform);
                    obj.transform.position = GetGridPos(i, j);
                    obj.SetActive(true);
                    obj.GetComponent<SpriteRenderer>().sortingOrder = i;

                    _beadControl[i, j] = new BeadController()
                    {
                        Row = i,
                        Column = j,
                        Color = color,
                        GameObject = obj
                    };
                }
            }
        }

        private Vector3 GetGridPos(int row, int column) => _grid.LocalToCell(new Vector3(row, column, 0));

        public class BeadController
        {
            public int Row;
            public int Column;
            public DemoGameBeads.DemoGameBeadsColor Color;
            public GameObject GameObject;
        }
    }
}