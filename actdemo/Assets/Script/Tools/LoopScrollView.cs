using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 这个类主要做了一件事,就是优化了,NGUI UIScrollView 在数据量很多都时候,
/// 创建过多都GameObject对象,造成资源浪费.
/// </summary>
public class LoopScrollView : MonoBehaviour
{
    public enum ArrangeDirection
    {
        Left_to_Right,
        Right_to_Left,
        Up_to_Down,
        Down_to_Up,
    }

    /// <summary>
    /// items的排列方式
    /// </summary>
    public ArrangeDirection arrangeDirection = ArrangeDirection.Up_to_Down;

    /// <summary>
    /// 列表单项模板
    /// </summary>
    public GameObject itemPrefab;

    /// <summary>
    /// 组件大小
    /// </summary>
    public Vector2 mItemSize;

    /// <summary>
    /// The items list.
    /// </summary>
    public List<LoopItemObject> itemsList;

    /// <summary>
    /// The datas list.
    /// </summary>
    public List<LoopItemData> datasList;

    /// <summary>
    /// 列表脚本
    /// </summary>
    public UIScrollView scrollView;

    /// <summary>
    /// 组件父节点
    /// </summary>
    public GameObject itemParent;

    /// <summary>
    /// itemsList的第一个元素
    /// </summary>
    LoopItemObject firstItem;

    /// <summary>
    /// itemsList的最后一个元素
    /// </summary>
    LoopItemObject lastItem;

    public delegate void DelegateHandler(LoopItemObject item, LoopItemData data);

    /// <summary>
    /// 响应
    /// </summary>
    public DelegateHandler OnItemInit;

    public delegate void DelegateOnClick(GameObject go);

    /// <summary>
    /// 点击组件事件
    /// </summary>
    public DelegateOnClick OnItemClick;

    /// <summary>
    /// 第一item的起始位置
    /// </summary>
    public Vector3 itemStartPos = Vector3.zero;

    /// <summary>
    /// 菜单项间隙
    /// </summary>
    public float gapDis = 0f;

    /// <summary>
    /// 每行个数限制
    /// </summary>
    public int mColLimit = 0;

    // 对象池
    // 再次优化，频繁的创建与销毁
    Queue<LoopItemObject> itemLoop = new Queue<LoopItemObject>();

    void Awake()
    {
        if (itemPrefab == null || scrollView == null || itemParent == null)
        {
            Debug.LogError("LoopScrollView.Awake() 有属性没有在inspector中赋值");
        }

        UIWidget widget = itemPrefab.GetComponent<UIWidget>();
        if (widget == null)
        {
            Debug.LogError("widget is null");

            return;
        }

        mItemSize = new Vector2(widget.width, widget.height);

        // 设置scrollview的movement
        if (arrangeDirection == ArrangeDirection.Up_to_Down ||
           arrangeDirection == ArrangeDirection.Down_to_Up)
        {
            scrollView.movement = UIScrollView.Movement.Vertical;
        }
        else
        {
            scrollView.movement = UIScrollView.Movement.Horizontal;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //if(scrollView.isDragging)
        {
            Validate();
        }
    }

    /// <summary>
    /// 检验items的两端是否要补上或删除
    /// </summary>
    void Validate()
    {
        if (datasList == null || datasList.Count == 0)
        {
            return;
        }

        // 如果itemsList还不存在
        if (itemsList == null || itemsList.Count == 0)
        {
            itemsList = new List<LoopItemObject>();

            LoopItemObject item = GetItemFromLoop();
            InitItem(item, 0, datasList[0]);
            firstItem = lastItem = item;
            itemsList.Add(item);

            //Validate();
        }

        // 
        bool all_invisible = true;
        foreach (LoopItemObject item in itemsList)
        {
            if (item.widget.isVisible == true)
            {
                all_invisible = false;
            }
        }
        if (all_invisible == true)
            return;

        // 先判断前端是否要增减
        if (firstItem.widget.isVisible)
        {
            // 判断要不要在它的前面补充一个item
            if (firstItem.dataIndex > 0)
            {
                LoopItemObject item = GetItemFromLoop();

                // 初化：数据索引、大小、位置、显示
                int index = firstItem.dataIndex - 1;
                //InitItem(item, index, datasList[index]);
                AddToFront(firstItem, item, index, datasList[index]);
                firstItem = item;
                itemsList.Insert(0, item);

                //Validate();
            }
        }
        else
        {
            // 判断要不要将它移除
            // 条件：自身是不可见的；且它后一个item也是不可见的（或被被裁剪过半的）.
            // 		这有个隐含条件是itemsList.Count>=2.
            if (itemsList.Count >= 2
               && itemsList[0].widget.isVisible == false
               && itemsList[1].widget.isVisible == false)
            {
                itemsList.Remove(firstItem);
                PutItemToLoop(firstItem);
                firstItem = itemsList[0];

                //Validate();
            }
        }

        // 再判断后端是否要增减
        if (lastItem.widget.isVisible)
        {
            // 判断要不要在它的后面补充一个item
            if (lastItem.dataIndex < datasList.Count - 1)
            {
                LoopItemObject item = GetItemFromLoop();

                // 初化：数据索引、大小、位置、显示
                int index = lastItem.dataIndex + 1;
                AddToBack(lastItem, item, index, datasList[index]);
                lastItem = item;
                itemsList.Add(item);

                //Validate();
            }
        }
        else
        {
            // 判断要不要将它移除
            // 条件：自身是不可见的；且它前一个item也是不可见的（或被被裁剪过半的）.
            // 		这有个隐含条件是itemsList.Count>=2.
            if (itemsList.Count >= 2
              && itemsList[itemsList.Count - 1].widget.isVisible == false
                  && itemsList[itemsList.Count - 2].widget.isVisible == false)
            {
                itemsList.Remove(lastItem);
                PutItemToLoop(lastItem);
                lastItem = itemsList[itemsList.Count - 1];

                //Validate();
            }
        }
    }

    /// <summary>
    /// Init the specified datas.
    /// </summary>
    /// <param name="datas">Datas.</param>
    public void Init(List<LoopItemData> datas, DelegateHandler onItemInitCallback, DelegateOnClick onItemClickCallback)
    {
        datasList = datas;
        this.OnItemInit = onItemInitCallback;
        this.OnItemClick = onItemClickCallback;

        Validate();
    }

    /// <summary>
    /// 构造一个 item 对象
    /// </summary>
    /// <returns>The item.</returns>
    LoopItemObject CreateItem()
    {
        GameObject go = NGUITools.AddChild(itemParent, itemPrefab);
        UIWidget widget = go.GetComponent<UIWidget>();
        LoopItemObject item = new LoopItemObject();
        item.widget = widget;
        go.SetActive(true);
        return item;
    }

    /// <summary>
    /// 用数据列表来初始化scrollview
    /// </summary>
    /// <param name="item">Item.</param>
    /// <param name="indexData">Index data.</param>
    /// <param name="data">Data.</param>
    void InitItem(LoopItemObject item, int dataIndex, LoopItemData data)
    {
        item.dataIndex = dataIndex;
        if (OnItemInit != null)
        {
            OnItemInit(item, data);
        }

        if (OnItemClick != null)
        {
            UIEventListener listen = item.widget.transform.GetComponent<UIEventListener>();
            if (listen == null)
            {
                listen = item.widget.transform.gameObject.AddComponent<UIEventListener>();
            }
            listen.onClick = OnItemClickFunc;
        }

        Transform trans = item.widget.transform;
        if (trans.GetComponent<UIDragScrollView>() == null)
        {
            UIDragScrollView drag = trans.gameObject.AddComponent<UIDragScrollView>();
            drag.scrollView = gameObject.GetComponent<UIScrollView>();
        }

        trans.localPosition = itemStartPos;

        if (!string.IsNullOrEmpty(data.mItemName))
        {
            item.widget.transform.name = data.mItemName;
        }
    }

    /// <summary>
    /// 在itemsList前面补上一个item
    /// </summary>
    void AddToFront(LoopItemObject priorItem, LoopItemObject newItem, int newIndex, LoopItemData newData)
    {
        InitItem(newItem, newIndex, newData);

        float offsetX = 0.0f;
        float offsetY = 0.0f;

        // 计算新item的位置
        if (scrollView.movement == UIScrollView.Movement.Vertical)
        {
            if (mColLimit < 1)
            {
                offsetY = mItemSize.y;

                SetNewItemPosition(priorItem, newItem, new Vector2(offsetX, offsetY));
            }
            else
            {
                int space = newIndex % mColLimit;
                if (space == 0 || space != mColLimit - 1)
                {
                    offsetX = mItemSize.x;

                    SetNewItemPosition(priorItem, newItem, new Vector2(-offsetX, offsetY));
                }
                else
                {
                    offsetX = (mColLimit - 1) * mItemSize.x;
                    offsetY = mItemSize.y;

                    SetNewItemPosition(priorItem, newItem, new Vector2(offsetX, offsetY));
                }
            }
        }
        else
        {
            if (mColLimit <1)
            {
                offsetX = mItemSize.x;

                SetNewItemPosition(priorItem, newItem, new Vector2(-offsetX, offsetY));
            }
            else
            {
                int space = newIndex % mColLimit;
                if (space == 0 || space != mColLimit - 1)
                {
                    offsetY = mItemSize.y;

                    SetNewItemPosition(priorItem, newItem, new Vector2(offsetX, offsetY));
                }
                else
                {
                    offsetX = mItemSize.x;
                    offsetY = (mColLimit - 1) * mItemSize.y;

                    SetNewItemPosition(priorItem, newItem, new Vector2(-offsetX, -offsetY));
                }
            }
        }
    }

    /// <summary>
    /// 在itemsList后面补上一个item
    /// </summary>
    void AddToBack(LoopItemObject backItem, LoopItemObject newItem, int newIndex, LoopItemData newData)
    {
        InitItem(newItem, newIndex, newData);

        float offsetX = 0.0f;
        float offsetY = 0.0f;

        // 计算新item的位置
        if (scrollView.movement == UIScrollView.Movement.Vertical)
        {
            if (mColLimit < 1)
            {
                offsetY = mItemSize.y;

                SetNewItemPosition(backItem, newItem, new Vector2(offsetX, -offsetY));
            }
            else
            {
                int space = newIndex % mColLimit;
                if (space == 0)
                {
                    offsetX = (mColLimit - 1) * mItemSize.x;
                    offsetY = mItemSize.y;

                    SetNewItemPosition(backItem, newItem, new Vector2(-offsetX, -offsetY));
                }
                else
                {
                    offsetX = mItemSize.x;

                    SetNewItemPosition(backItem, newItem, new Vector2(offsetX, offsetY));
                }
            }
        }
        else
        {
            if (mColLimit < 1)
            {
                offsetX = mItemSize.x;

                SetNewItemPosition(backItem, newItem, new Vector2(offsetX, offsetY));
            }
            else
            {
                int space = newIndex % mColLimit;
                if (space == 0)
                {
                    offsetX = mItemSize.x;
                    offsetY = (mColLimit - 1) * mItemSize.y;

                    SetNewItemPosition(backItem, newItem, new Vector2(offsetX, offsetY));
                }
                else
                {
                    offsetY = mItemSize.y;

                    SetNewItemPosition(backItem, newItem, new Vector2(offsetX, -offsetY));
                }
            }
        }
    }

    void SetNewItemPosition(LoopItemObject oldItem, LoopItemObject newItem, Vector2 pos)
    {
        newItem.widget.transform.localPosition = oldItem.widget.cachedTransform.localPosition + new Vector3(pos.x, pos.y, .0f);
    }

    /// <summary>
    /// 点击事件触发
    /// </summary>
    /// <param name="go"></param>
    void OnItemClickFunc(GameObject go)
    {
        if (OnItemClick != null)
        {
            OnItemClick(go);
        }
    }

    #region 对象池性能相关
    /// <summary>
    /// 从对象池中取行一个item
    /// </summary>
    /// <returns>The item from loop.</returns>
    LoopItemObject GetItemFromLoop()
    {
        LoopItemObject item;
        if (itemLoop.Count <= 0)
        {
            item = CreateItem();
        }
        else
        {
            item = itemLoop.Dequeue();
        }

        item.widget.gameObject.SetActive(true);

        return item;
    }

    /// <summary>
    /// 将要移除的item放入对象池中
    /// --这个里我保证这个对象池中存在的对象不超过3个
    /// </summary>
    /// <param name="item">Item.</param>
    void PutItemToLoop(LoopItemObject item)
    {
        if (itemLoop.Count >= mColLimit)
        {
            Destroy(item.widget.gameObject);
            return;
        }

        item.dataIndex = -1;
        item.widget.gameObject.SetActive(false);
        itemLoop.Enqueue(item);
    }
    #endregion

}