using System.Collections;
using System.Diagnostics;

namespace LibTNT;

internal sealed class SpatialTNTList : IReadOnlyCollection<TNT>
{
	private readonly SortedDictionary<Vec3B, SortedList<uint, TNT>> groups = new();
	private readonly LinkedList<TNT> inOrder;

	public int Count => inOrder.Count;

	public SpatialTNTList(IEnumerable<TNT> tnt)
	{
		foreach (TNT x in tnt)
		{
			x.spatialBucket = Vec3B.FromPosition(x.position);
			if (groups.TryGetValue(x.spatialBucket, out SortedList<uint, TNT>? list))
			{
				list.Add(x.order, x);
			}
			else
			{
				groups[x.spatialBucket] = new()
				{
					{ x.order, x }
				};
			}
		}
		inOrder = new(tnt.OrderBy(static x => x.order));
	}

	public void MoveTo(TNT tnt, Vec3B to)
	{
		if (!groups.TryGetValue(tnt.spatialBucket, out SortedList<uint, TNT>? fromList))
		{
			return;
		}
		if (!fromList.Remove(tnt.order))
		{
			return;
		}
		if (fromList.Count == 0)
		{
			fromList.TrimExcess();
		}

		if (tnt.Removed)
		{
			return;
		}

		if (!groups.TryGetValue(to, out SortedList<uint, TNT>? toList))
		{
			groups[to] = new()
			{
				{ tnt.order, tnt }
			};
			return;
		}
		toList.Add(tnt.order, tnt);
	}

	public void ModifyInBucket(Vec3B bucket, TNTModifier func)
	{
		if (!groups.TryGetValue(bucket, out SortedList<uint, TNT>? list))
		{
			return;
		}
		for (int i = 0; i < list.Count; i++)
		{
			TNT tnt = list.Values[i];
			if (!tnt.Removed)
			{
				func(tnt);
			}
		}
	}

	public void ModifyInOrder(TNTModifier func)
	{
		LinkedListNode<TNT>? n = inOrder.First;
		while (n is not null)
		{
			func(n.Value);
			n = n.Next;
		}
	}

	public void RemoveExploded()
	{
		LinkedListNode<TNT>? n = inOrder.First;
		while (n is not null)
		{
			if (n.Value.Removed)
			{
				LinkedListNode<TNT>? next = n.Next;
				inOrder.Remove(n);
				if (groups.TryGetValue(n.Value.spatialBucket, out SortedList<uint, TNT>? list))
				{
					Debug.Assert(list.Remove(n.Value.order));
					if (list.Count == 0)
					{
						list.TrimExcess();
					}
				}
				n = next;
			}
			else
			{
				n = n.Next;
			}
		}
	}

	public IEnumerator<TNT> GetEnumerator() => inOrder.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => inOrder.GetEnumerator();
}