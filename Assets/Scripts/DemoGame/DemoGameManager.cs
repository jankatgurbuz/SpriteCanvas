using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Manager;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DemoGame
{
    public class DemoGameManager : MonoBehaviour
    {
        [SerializeField] private int _rowLength;
        [SerializeField] private int _columnLength;
        [SerializeField] private Material _material;
        [SerializeField] private GameObject _hand;

        private Grid _grid;
        private DemoGameBeads _beads;

        private BeadController[,] _beadControl;

        private void Awake()
        {
            _grid = GetComponent<Grid>();
            _beads = GetComponent<DemoGameBeads>();
            Create();
            StartCoroutine(Loop());
        }

        private void Create()
        {
            _beadControl = new BeadController[_rowLength, _columnLength];
            for (int i = 0; i < _rowLength; i++)
            {
                for (int j = 0; j < _columnLength; j++)
                {
                    var (obj, color) = _beads.GetRandomObject();
                    obj = Instantiate(obj, transform);
                    obj.transform.position = GetGridPos(i, j);
                    obj.SetActive(true);
                    obj.GetComponent<SpriteRenderer>().sortingOrder = i;
                    obj.GetComponent<SpriteRenderer>().material = _material;

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

        private IEnumerator Loop()
        {
            int count = 0;
            while (true)
            {
                while (CheckPop(out List<BeadController> beadControllers))
                {
                    yield return new WaitForSeconds(0.25f);
                    yield return StartCoroutine(HandMovement(beadControllers.First()));
                    Pop(beadControllers);
                    yield return new WaitForEndOfFrame();
                }

                count++;

                if (count == 300)
                {
                    foreach (var item in _beadControl)
                    {
                        if (item.GameObject != null)
                        {
                            Destroy(item.GameObject);
                        }
                    }

                    Awake();
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator HandMovement(BeadController first)
        {
            while (true)
            {
                var targetPosition = GetGridPos(first.Row, first.Column);
                var normal = (targetPosition - _hand.transform.position).normalized;
                _hand.transform.position += normal * (Time.deltaTime * 10);
                if (Vector3.Distance(_hand.transform.position, targetPosition) < 0.01f)
                {
                    _hand.transform.position = targetPosition;
                    yield return new WaitForSeconds(0.25f);

                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private IEnumerator MovementUI(GameObject obj, DemoGameBeads.DemoGameBeadsColor tempColor, int index)
        {
            yield return new WaitForSeconds(index * 0.1f);
            if (obj == null) yield break;

            obj.GetComponent<SpriteRenderer>().sortingOrder = 1_000;
            while (true)
            {
                if (obj == null) yield break;
                var name = tempColor.ToString();
                var targetPosition = SpriteCanvasManager.Instance.GetTarget(name).transform.position;
                var normal = (targetPosition - obj.transform.position).normalized;
                obj.transform.position += normal * (Time.deltaTime * 10);
                if (Vector3.Distance(obj.transform.position, targetPosition) < 0.01f)
                {
                    obj.transform.position = targetPosition;
                    Destroy(obj);
                    break;
                }

                yield return new WaitForEndOfFrame();
            }
        }

        private void Pop(List<BeadController> beadControllers)
        {
            for (var index = 0; index < beadControllers.Count; index++)
            {
                var item = beadControllers[index];
                int row = item.Row;
                int column = item.Column;
                var tempColor = _beadControl[row, column].Color;
                _beadControl[row, column].Color = DemoGameBeads.DemoGameBeadsColor.Empty;
                StartCoroutine(MovementUI(_beadControl[row, column].GameObject, tempColor, index));
            }
        }

        private bool CheckPop(out List<BeadController> beadControllers)
        {
            var randomRow = Random.Range(0, _rowLength);
            var randomColumn = Random.Range(0, _columnLength);

            var checker = new bool[_rowLength, _columnLength];
            beadControllers = new List<BeadController>();

            var temp = _beadControl[randomRow, randomColumn];
            CheckPop(temp.Row, temp.Column, temp.Color, checker, beadControllers);

            return beadControllers.Count > 1;
        }

        private void CheckPop(int row, int column, DemoGameBeads.DemoGameBeadsColor color, bool[,] checker,
            List<BeadController> elements)
        {
            if (row < 0 ||
                row == _rowLength ||
                column < 0 || column == _columnLength ||
                checker[row, column] ||
                color != _beadControl[row, column].Color ||
                color == DemoGameBeads.DemoGameBeadsColor.Empty)
            {
                return;
            }

            checker[row, column] = true;
            elements.Add(_beadControl[row, column]);

            CheckPop(row + 1, column, color, checker, elements);
            CheckPop(row - 1, column, color, checker, elements);

            CheckPop(row, column + 1, color, checker, elements);
            CheckPop(row, column - 1, color, checker, elements);
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