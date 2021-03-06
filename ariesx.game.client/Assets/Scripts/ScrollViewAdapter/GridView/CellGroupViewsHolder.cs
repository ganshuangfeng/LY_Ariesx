using frame8.Logic.Misc.Visual.UI.ScrollRectItemsAdapter;
using Poukoute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace frame8.ScrollRectItemsAdapter.Util.GridView {
    /// <summary>
    /// <para>A views holder representing a group of cells (row or column). It instantiates the maximum number of cells it can contain,</para>
    /// <para>but only those of them that should be displayed will have their <see cref="CellViewsHolder.views"/> enabled</para>
    /// </summary>
    /// <typeparam name="TCellVH">The views holder type used for the cells in this group</typeparam>
    public class CellGroupViewsHolder : BaseItemViewsHolder {
        /// <summary>Uses base's implementation, but also updates the indices of all containing cells each time the setter is called</summary>
        public override int ItemIndex {
            get { return base.ItemIndex; }
            set {
                base.ItemIndex = value;
                if (_Capacity > 0) {
                    for (int i = 0; i < _Capacity; ++i) {
                        ContainingCellViewsHolders[i].ItemIndex = ItemIndex * _Capacity + i;
                    }
                }
            }
        }

        /// <summary>The number of visible cells, i.e. that are used to display real data. The other ones are disabled and are either empty or hold obsolete data</summary>
        public int NumActiveCells {
            get { return _NumActiveCells; }
            set {
                if (_NumActiveCells != value) {
                    _NumActiveCells = value;
                    for (int i = 0; i < _Capacity; ++i)
                        ContainingCellViewsHolders[i].gameObject.SetActive(i < _NumActiveCells);
                }
            }
        }

        /// <summary>The views holders of all containing cells, active or not</summary>
        public BaseItemViewsHolder[] ContainingCellViewsHolders { get; private set; }

        protected int _Capacity = -1;
        protected int _NumActiveCells = 0;

        /// <summary>The only way to instantiate the group views holder. It's used internally, since the group managing is not done by the API user</summary>
        /// <param name="itemIndex">the group's index</param>
        internal void Init(GameObject groupPrefab, int itemIndex,
            string cellPrefabPath, int numCellsPerGroup) {
            root = groupPrefab.transform as RectTransform;
            //root.localEulerAngles = Vector3.zero;
            //root.sizeDelta = new Vector2((content as RectTransform).sizeDelta.x, defaultItemSize);
            root.gameObject.SetActive(true);

            this.ItemIndex = itemIndex;
            _Capacity = numCellsPerGroup;
            // Instantiate all the cells in the group
            ContainingCellViewsHolders = new BaseItemViewsHolder[_Capacity];
            for (int i = 0; i < _Capacity; ++i) {
                GameObject cellInstance = PoolManager.GetObject(cellPrefabPath, root);
                cellInstance.SetActive(true);
                ContainingCellViewsHolders[i] = cellInstance.GetComponent<BaseItemViewsHolder>();
            }

        }
    }

}
