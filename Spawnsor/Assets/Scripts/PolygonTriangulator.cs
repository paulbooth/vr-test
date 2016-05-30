using UnityEngine;
using System.Collections.Generic;

public class PolygonTriangulator
{
	/// <summary>
	/// Calculate list of convex polygons or triangles.
	/// </summary>
	/// <param name="Polygon">Input polygon without self-intersections (it can be checked with SelfIntersection().</param>
	/// <param name="triangulate">true: splitting on triangles; false: splitting on convex polygons.</param>
	/// <returns></returns>
	public static List<List<Vector2>> Triangulate(List<Vector2> Polygon, bool triangulate = false)
	{
		var result = new List<List<Vector2>>();
		var tempPolygon = new List<Vector2>(Polygon);
		var convPolygon = new List<Vector2>();

		int begin_ind = 0;
		int cur_ind;
		int begin_ind1;
		int N = Polygon.Count;
		int Range;

		if (Square(tempPolygon) < 0)
			tempPolygon.Reverse();

		while (N >= 3)
		{
			while ((PMSquare(tempPolygon[begin_ind], tempPolygon[(begin_ind + 1) % N],
				tempPolygon[(begin_ind + 2) % N]) < 0) ||
				(Intersect(tempPolygon, begin_ind, (begin_ind + 1) % N, (begin_ind + 2) % N) == true))
			{
				begin_ind++;
				begin_ind %= N;
			}
			cur_ind = (begin_ind + 1) % N;
			convPolygon.Add(tempPolygon[begin_ind]);
			convPolygon.Add(tempPolygon[cur_ind]);
			convPolygon.Add(tempPolygon[(begin_ind + 2) % N]);

			if (triangulate == false)
			{
				begin_ind1 = cur_ind;
				while ((PMSquare(tempPolygon[cur_ind], tempPolygon[(cur_ind + 1) % N],
					tempPolygon[(cur_ind + 2) % N]) > 0) && ((cur_ind + 2) % N != begin_ind))
				{
					if ((Intersect(tempPolygon, begin_ind, (cur_ind + 1) % N, (cur_ind + 2) % N) == true) ||
						(PMSquare(tempPolygon[begin_ind], tempPolygon[(begin_ind + 1) % N],
							tempPolygon[(cur_ind + 2) % N]) < 0))
						break;
					convPolygon.Add(tempPolygon[(cur_ind + 2) % N]);
					cur_ind++;
					cur_ind %= N;
				}
			}

			Range = cur_ind - begin_ind;
			if (Range > 0)
			{
				tempPolygon.RemoveRange(begin_ind + 1, Range);
			}
			else
			{
				tempPolygon.RemoveRange(begin_ind + 1, N - begin_ind - 1);
				tempPolygon.RemoveRange(0, cur_ind + 1);
			}
			N = tempPolygon.Count;
			begin_ind++;
			begin_ind %= N;

			result.Add(convPolygon);
		}

		return result;
	}

	public static int SelfIntersection(List<Vector2> polygon)
	{
		if (polygon.Count < 3)
			return 0;
		int High = polygon.Count - 1;
		Vector2 O = new Vector2();
		int i;
		for (i = 0; i < High; i++)
		{
			for (int j = i + 2; j < High; j++)
			{
				if (LineIntersect(polygon[i], polygon[i + 1],
					polygon[j], polygon[j + 1], ref O) == 1)
					return 1;
			}
		}
		for (i = 1; i < High - 1; i++)
			if (LineIntersect(polygon[i], polygon[i + 1], polygon[High], polygon[0], ref O) == 1)
				return 1;
		return -1;
	}

	public static float Square(List<Vector2> polygon)
	{
		float S = 0;
		if (polygon.Count >= 3)
		{
			for (int i = 0; i < polygon.Count - 1; i++)
				S += PMSquare((Vector2)polygon[i], (Vector2)polygon[i + 1]);
			S += PMSquare((Vector2)polygon[polygon.Count - 1], (Vector2)polygon[0]);
		}
		return S;
	}

	public static int IsConvex(List<Vector2> Polygon)
	{
		if (Polygon.Count >= 3)
		{
			if (Square(Polygon) > 0)
			{
				for (int i = 0; i < Polygon.Count - 2; i++)
					if (PMSquare(Polygon[i], Polygon[i + 1], Polygon[i + 2]) < 0)
						return -1;
				if (PMSquare(Polygon[Polygon.Count - 2], Polygon[Polygon.Count - 1], Polygon[0]) < 0)
					return -1;
				if (PMSquare(Polygon[Polygon.Count - 1], Polygon[0], Polygon[1]) < 0)
					return -1;
			}
			else
			{
				for (int i = 0; i < Polygon.Count - 2; i++)
					if (PMSquare(Polygon[i], Polygon[i + 1], Polygon[i + 2]) > 0)
						return -1;
				if (PMSquare(Polygon[Polygon.Count - 2], Polygon[Polygon.Count - 1], Polygon[0]) > 0)
					return -1;
				if (PMSquare(Polygon[Polygon.Count - 1], Polygon[0], Polygon[1]) > 0)
					return -1;
			}
			return 1;
		}
		return 0;
	}

	static bool Intersect(List<Vector2> polygon, int vertex1Ind, int vertex2Ind, int vertex3Ind)
	{
		float s1, s2, s3;
		for (int i = 0; i < polygon.Count; i++)
		{
			if ((i == vertex1Ind) || (i == vertex2Ind) || (i == vertex3Ind))
				continue;
			s1 = PMSquare(polygon[vertex1Ind], polygon[vertex2Ind], polygon[i]);
			s2 = PMSquare(polygon[vertex2Ind], polygon[vertex3Ind], polygon[i]);
			if (((s1 < 0) && (s2 > 0)) || ((s1 > 0) && (s2 < 0)))
				continue;
			s3 = PMSquare(polygon[vertex3Ind], polygon[vertex1Ind], polygon[i]);
			if (((s3 >= 0) && (s2 >= 0)) || ((s3 <= 0) && (s2 <= 0)))
				return true;
		}
		return false;
	}

	static float PMSquare(Vector2 p1, Vector2 p2)
	{
		return (p2.x * p1.y - p1.x * p2.y);
	}

	static float PMSquare(Vector2 p1, Vector2 p2, Vector2 p3)
	{
		return (p3.x - p1.x) * (p2.y - p1.y) - (p2.x - p1.x) * (p3.y - p1.y);
	}

	static int LineIntersect(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2, ref Vector2 O)
	{
		float a1 = A2.y - A1.y;
		float b1 = A1.x - A2.x;
		float d1 = -a1 * A1.x - b1 * A1.y;
		float a2 = B2.y - B1.y;
		float b2 = B1.x - B2.x;
		float d2 = -a2 * B1.x - b2 * B1.y;
		float t = a2 * b1 - a1 * b2;

		if (t == 0)
			return -1;

		O.y = (a1 * d2 - a2 * d1) / t;
		O.x = (b2 * d1 - b1 * d2) / t;

		if (A1.x > A2.x)
		{
			if ((O.x < A2.x) || (O.x > A1.x))
				return 0;
		}
		else
		{
			if ((O.x < A1.x) || (O.x > A2.x))
				return 0;
		}

		if (A1.y > A2.y)
		{
			if ((O.y < A2.y) || (O.y > A1.y))
				return 0;
		}
		else
		{
			if ((O.y < A1.y) || (O.y > A2.y))
				return 0;
		}

		if (B1.x > B2.x)
		{
			if ((O.x < B2.x) || (O.x > B1.x))
				return 0;
		}
		else
		{
			if ((O.x < B1.x) || (O.x > B2.x))
				return 0;
		}

		if (B1.y > B2.y)
		{
			if ((O.y < B2.y) || (O.y > B1.y))
				return 0;
		}
		else
		{
			if ((O.y < B1.y) || (O.y > B2.y))
				return 0;
		}

		return 1;
	}
}