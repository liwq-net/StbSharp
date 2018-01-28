// Generated by Sichem at 1/28/2018 12:47:33 PM

using System;
using System.Runtime.InteropServices;

namespace StbSharp
{
	unsafe partial class StbDxt
	{
		public static byte[] stb__Expand5 = new byte[32];
		public static byte[] stb__Expand6 = new byte[64];
		public static byte[] stb__OMatch5 = new byte[512];
		public static byte[] stb__OMatch6 = new byte[512];
		public static byte[] stb__QuantRBTab = new byte[256 + 16];
		public static byte[] stb__QuantGTab = new byte[256 + 16];
		public static int init = (int) (1);

		public static int stb__Mul8Bit(int a, int b)
		{
			int t = (int) (a*b + 128);
			return (int) ((t + (t >> 8)) >> 8);
		}

		public static void stb__From16Bit(byte* _out_, ushort v)
		{
			int rv = (int) ((v & 0xf800) >> 11);
			int gv = (int) ((v & 0x07e0) >> 5);
			int bv = (int) ((v & 0x001f) >> 0);
			_out_[0] = (byte) (stb__Expand5[rv]);
			_out_[1] = (byte) (stb__Expand6[gv]);
			_out_[2] = (byte) (stb__Expand5[bv]);
			_out_[3] = (byte) (0);
		}

		public static ushort stb__As16Bit(int r, int g, int b)
		{
			return
				(ushort)
					((stb__Mul8Bit((int) (r), (int) (31)) << 11) + (stb__Mul8Bit((int) (g), (int) (63)) << 5) +
					 stb__Mul8Bit((int) (b), (int) (31)));
		}

		public static int stb__Lerp13(int a, int b)
		{
			return (int) ((2*a + b)/3);
		}

		public static void stb__Lerp13RGB(byte* _out_, byte* p1, byte* p2)
		{
			_out_[0] = (byte) (stb__Lerp13((int) (p1[0]), (int) (p2[0])));
			_out_[1] = (byte) (stb__Lerp13((int) (p1[1]), (int) (p2[1])));
			_out_[2] = (byte) (stb__Lerp13((int) (p1[2]), (int) (p2[2])));
		}

		public static void stb__PrepareOptTable(byte[] Table, byte[] expand, int size)
		{
			int i;
			int mn;
			int mx;
			for (i = (int) (0); (i) < (256); i++)
			{
				int bestErr = (int) (256);
				for (mn = (int) (0); (mn) < (size); mn++)
				{
					for (mx = (int) (0); (mx) < (size); mx++)
					{
						int mine = (int) (expand[mn]);
						int maxe = (int) (expand[mx]);
						int err = (int) (CRuntime.abs((int) (stb__Lerp13((int) (maxe), (int) (mine)) - i)));
						err += (int) (CRuntime.abs((int) (maxe - mine))*3/100);
						if ((err) < (bestErr))
						{
							Table[i*2 + 0] = (byte) (mx);
							Table[i*2 + 1] = (byte) (mn);
							bestErr = (int) (err);
						}
					}
				}
			}
		}

		public static void stb__EvalColors(byte* color, ushort c0, ushort c1)
		{
			stb__From16Bit(color + 0, (ushort) (c0));
			stb__From16Bit(color + 4, (ushort) (c1));
			stb__Lerp13RGB(color + 8, color + 0, color + 4);
			stb__Lerp13RGB(color + 12, color + 4, color + 0);
		}

		public static uint stb__MatchColorsBlock(byte* block, byte* color, int dither)
		{
			uint mask = (uint) (0);
			int dirr = (int) (color[0*4 + 0] - color[1*4 + 0]);
			int dirg = (int) (color[0*4 + 1] - color[1*4 + 1]);
			int dirb = (int) (color[0*4 + 2] - color[1*4 + 2]);
			int* dots = stackalloc int[16];
			int* stops = stackalloc int[4];
			int i;
			int c0Point;
			int halfPoint;
			int c3Point;
			for (i = (int) (0); (i) < (16); i++)
			{
				dots[i] = (int) (block[i*4 + 0]*dirr + block[i*4 + 1]*dirg + block[i*4 + 2]*dirb);
			}
			for (i = (int) (0); (i) < (4); i++)
			{
				stops[i] = (int) (color[i*4 + 0]*dirr + color[i*4 + 1]*dirg + color[i*4 + 2]*dirb);
			}
			c0Point = (int) ((stops[1] + stops[3]) >> 1);
			halfPoint = (int) ((stops[3] + stops[2]) >> 1);
			c3Point = (int) ((stops[2] + stops[0]) >> 1);
			if (dither == 0)
			{
				for (i = (int) (15); (i) >= (0); i--)
				{
					int dot = (int) (dots[i]);
					mask <<= 2;
					if ((dot) < (halfPoint)) mask |= (uint) (((dot) < (c0Point)) ? 1 : 3);
					else mask |= (uint) (((dot) < (c3Point)) ? 2 : 0);
				}
			}
			else
			{
				int* err = stackalloc int[8];
				int* ep1 = err;
				int* ep2 = err + 4;
				int* dp = dots;
				int y;
				c0Point <<= 4;
				halfPoint <<= 4;
				c3Point <<= 4;
				for (i = (int) (0); (i) < (8); i++)
				{
					err[i] = (int) (0);
				}
				for (y = (int) (0); (y) < (4); y++)
				{
					int dot;
					int lmask;
					int step;
					dot = (int) ((dp[0] << 4) + (3*ep2[1] + 5*ep2[0]));
					if ((dot) < (halfPoint)) step = (int) (((dot) < (c0Point)) ? 1 : 3);
					else step = (int) (((dot) < (c3Point)) ? 2 : 0);
					ep1[0] = (int) (dp[0] - stops[step]);
					lmask = (int) (step);
					dot = (int) ((dp[1] << 4) + (7*ep1[0] + 3*ep2[2] + 5*ep2[1] + ep2[0]));
					if ((dot) < (halfPoint)) step = (int) (((dot) < (c0Point)) ? 1 : 3);
					else step = (int) (((dot) < (c3Point)) ? 2 : 0);
					ep1[1] = (int) (dp[1] - stops[step]);
					lmask |= (int) (step << 2);
					dot = (int) ((dp[2] << 4) + (7*ep1[1] + 3*ep2[3] + 5*ep2[2] + ep2[1]));
					if ((dot) < (halfPoint)) step = (int) (((dot) < (c0Point)) ? 1 : 3);
					else step = (int) (((dot) < (c3Point)) ? 2 : 0);
					ep1[2] = (int) (dp[2] - stops[step]);
					lmask |= (int) (step << 4);
					dot = (int) ((dp[3] << 4) + (7*ep1[2] + 5*ep2[3] + ep2[2]));
					if ((dot) < (halfPoint)) step = (int) (((dot) < (c0Point)) ? 1 : 3);
					else step = (int) (((dot) < (c3Point)) ? 2 : 0);
					ep1[3] = (int) (dp[3] - stops[step]);
					lmask |= (int) (step << 6);
					dp += 4;
					mask |= (uint) (lmask << (y*8));
					{
						int* et = ep1;
						ep1 = ep2;
						ep2 = et;
					}
				}
			}

			return (uint) (mask);
		}

		public static void stb__OptimizeColorsBlock(byte* block, ushort* pmax16, ushort* pmin16)
		{
			int mind = (int) (0x7fffffff);
			int maxd = (int) (-0x7fffffff);
			byte* minp = null;
			byte* maxp = null;
			double magn;
			int v_r;
			int v_g;
			int v_b;
			int nIterPower = (int) (4);
			float* covf = stackalloc float[6];
			float vfr;
			float vfg;
			float vfb;
			int* cov = stackalloc int[6];
			int* mu = stackalloc int[3];
			int* min = stackalloc int[3];
			int* max = stackalloc int[3];
			int ch;
			int i;
			int iter;
			for (ch = (int) (0); (ch) < (3); ch++)
			{
				byte* bp = (block) + ch;
				int muv;
				int minv;
				int maxv;
				muv = (int) (minv = (int) (maxv = (int) (bp[0])));
				for (i = (int) (4); (i) < (64); i += (int) (4))
				{
					muv += (int) (bp[i]);
					if ((bp[i]) < (minv)) minv = (int) (bp[i]);
					else if ((bp[i]) > (maxv)) maxv = (int) (bp[i]);
				}
				mu[ch] = (int) ((muv + 8) >> 4);
				min[ch] = (int) (minv);
				max[ch] = (int) (maxv);
			}
			for (i = (int) (0); (i) < (6); i++)
			{
				cov[i] = (int) (0);
			}
			for (i = (int) (0); (i) < (16); i++)
			{
				int r = (int) (block[i*4 + 0] - mu[0]);
				int g = (int) (block[i*4 + 1] - mu[1]);
				int b = (int) (block[i*4 + 2] - mu[2]);
				cov[0] += (int) (r*r);
				cov[1] += (int) (r*g);
				cov[2] += (int) (r*b);
				cov[3] += (int) (g*g);
				cov[4] += (int) (g*b);
				cov[5] += (int) (b*b);
			}
			for (i = (int) (0); (i) < (6); i++)
			{
				covf[i] = (float) (cov[i]/255.0f);
			}
			vfr = ((float) (max[0] - min[0]));
			vfg = ((float) (max[1] - min[1]));
			vfb = ((float) (max[2] - min[2]));
			for (iter = (int) (0); (iter) < (nIterPower); iter++)
			{
				float r = (float) (vfr*covf[0] + vfg*covf[1] + vfb*covf[2]);
				float g = (float) (vfr*covf[1] + vfg*covf[3] + vfb*covf[4]);
				float b = (float) (vfr*covf[2] + vfg*covf[4] + vfb*covf[5]);
				vfr = (float) (r);
				vfg = (float) (g);
				vfb = (float) (b);
			}
			magn = (double) (CRuntime.fabs((double) (vfr)));
			if ((CRuntime.fabs((double) (vfg))) > (magn)) magn = (double) (CRuntime.fabs((double) (vfg)));
			if ((CRuntime.fabs((double) (vfb))) > (magn)) magn = (double) (CRuntime.fabs((double) (vfb)));
			if ((magn) < (4.0f))
			{
				v_r = (int) (299);
				v_g = (int) (587);
				v_b = (int) (114);
			}
			else
			{
				magn = (double) (512.0/magn);
				v_r = ((int) (vfr*magn));
				v_g = ((int) (vfg*magn));
				v_b = ((int) (vfb*magn));
			}

			for (i = (int) (0); (i) < (16); i++)
			{
				int dot = (int) (block[i*4 + 0]*v_r + block[i*4 + 1]*v_g + block[i*4 + 2]*v_b);
				if ((dot) < (mind))
				{
					mind = (int) (dot);
					minp = block + i*4;
				}
				if ((dot) > (maxd))
				{
					maxd = (int) (dot);
					maxp = block + i*4;
				}
			}
			*pmax16 = (ushort) (stb__As16Bit((int) (maxp[0]), (int) (maxp[1]), (int) (maxp[2])));
			*pmin16 = (ushort) (stb__As16Bit((int) (minp[0]), (int) (minp[1]), (int) (minp[2])));
		}

		public static int stb__sclamp(float y, int p0, int p1)
		{
			int x = (int) (y);
			if ((x) < (p0)) return (int) (p0);
			if ((x) > (p1)) return (int) (p1);
			return (int) (x);
		}

		public static int stb__RefineBlock(byte* block, ushort* pmax16, ushort* pmin16, uint mask)
		{
			int* w1Tab = stackalloc int[4];
			w1Tab[0] = (int) (3);
			w1Tab[1] = (int) (0);
			w1Tab[2] = (int) (2);
			w1Tab[3] = (int) (1);

			int* prods = stackalloc int[4];
			prods[0] = (int) (0x090000);
			prods[1] = (int) (0x000900);
			prods[2] = (int) (0x040102);
			prods[3] = (int) (0x010402);

			float frb;
			float fg;
			ushort oldMin;
			ushort oldMax;
			ushort min16;
			ushort max16;
			int i;
			int akku = (int) (0);
			int xx;
			int xy;
			int yy;
			int At1_r;
			int At1_g;
			int At1_b;
			int At2_r;
			int At2_g;
			int At2_b;
			uint cm = (uint) (mask);
			oldMin = (ushort) (*pmin16);
			oldMax = (ushort) (*pmax16);
			if ((mask ^ (mask << 2)) < (4))
			{
				int r = (int) (8);
				int g = (int) (8);
				int b = (int) (8);
				for (i = (int) (0); (i) < (16); ++i)
				{
					r += (int) (block[i*4 + 0]);
					g += (int) (block[i*4 + 1]);
					b += (int) (block[i*4 + 2]);
				}
				r >>= 4;
				g >>= 4;
				b >>= 4;
				max16 = (ushort) ((stb__OMatch5[r] << 11) | (stb__OMatch6[g] << 5) | stb__OMatch5[b]);
				min16 = (ushort) ((stb__OMatch5[r + 256] << 11) | (stb__OMatch6[g + 256] << 5) | stb__OMatch5[b + 256]);
			}
			else
			{
				At1_r = (int) (At1_g = (int) (At1_b = (int) (0)));
				At2_r = (int) (At2_g = (int) (At2_b = (int) (0)));
				for (i = (int) (0); (i) < (16); ++i , cm >>= 2)
				{
					int step = (int) (cm & 3);
					int w1 = (int) (w1Tab[step]);
					int r = (int) (block[i*4 + 0]);
					int g = (int) (block[i*4 + 1]);
					int b = (int) (block[i*4 + 2]);
					akku += (int) (prods[step]);
					At1_r += (int) (w1*r);
					At1_g += (int) (w1*g);
					At1_b += (int) (w1*b);
					At2_r += (int) (r);
					At2_g += (int) (g);
					At2_b += (int) (b);
				}
				At2_r = (int) (3*At2_r - At1_r);
				At2_g = (int) (3*At2_g - At1_g);
				At2_b = (int) (3*At2_b - At1_b);
				xx = (int) (akku >> 16);
				yy = (int) ((akku >> 8) & 0xff);
				xy = (int) ((akku >> 0) & 0xff);
				frb = (float) (3.0f*31.0f/255.0f/(xx*yy - xy*xy));
				fg = (float) (frb*63.0f/31.0f);
				max16 = (ushort) (stb__sclamp((float) ((At1_r*yy - At2_r*xy)*frb + 0.5f), (int) (0), (int) (31)) << 11);
				max16 |= (ushort) (stb__sclamp((float) ((At1_g*yy - At2_g*xy)*fg + 0.5f), (int) (0), (int) (63)) << 5);
				max16 |= (ushort) (stb__sclamp((float) ((At1_b*yy - At2_b*xy)*frb + 0.5f), (int) (0), (int) (31)) << 0);
				min16 = (ushort) (stb__sclamp((float) ((At2_r*xx - At1_r*xy)*frb + 0.5f), (int) (0), (int) (31)) << 11);
				min16 |= (ushort) (stb__sclamp((float) ((At2_g*xx - At1_g*xy)*fg + 0.5f), (int) (0), (int) (63)) << 5);
				min16 |= (ushort) (stb__sclamp((float) ((At2_b*xx - At1_b*xy)*frb + 0.5f), (int) (0), (int) (31)) << 0);
			}

			*pmin16 = (ushort) (min16);
			*pmax16 = (ushort) (max16);
			return (int) ((oldMin != min16) || (oldMax != max16) ? 1 : 0);
		}

		public static void stb__CompressColorBlock(byte* dest, byte* block, int mode)
		{
			uint mask;
			int i;
			int dither;
			int refinecount;
			ushort max16;
			ushort min16;
			byte* dblock = stackalloc byte[16*4];
			byte* color = stackalloc byte[4*4];
			dither = (int) (mode & 1);
			refinecount = (int) ((mode & 2) != 0 ? 2 : 1);
			for (i = (int) (1); (i) < (16); i++)
			{
				if (((uint*) (block))[i] != ((uint*) (block))[0]) break;
			}
			if ((i) == (16))
			{
				int r = (int) (block[0]);
				int g = (int) (block[1]);
				int b = (int) (block[2]);
				mask = (uint) (0xaaaaaaaa);
				max16 = (ushort) ((stb__OMatch5[r] << 11) | (stb__OMatch6[g] << 5) | stb__OMatch5[b]);
				min16 = (ushort) ((stb__OMatch5[r + 256] << 11) | (stb__OMatch6[g + 256] << 5) | stb__OMatch5[b + 256]);
			}
			else
			{
				if ((dither) != 0) stb__DitherBlock(dblock, block);
				stb__OptimizeColorsBlock((dither) != 0 ? dblock : block, &max16, &min16);
				if (max16 != min16)
				{
					stb__EvalColors(color, (ushort) (max16), (ushort) (min16));
					mask = (uint) (stb__MatchColorsBlock(block, color, (int) (dither)));
				}
				else mask = (uint) (0);
				for (i = (int) (0); (i) < (refinecount); i++)
				{
					uint lastmask = (uint) (mask);
					if ((stb__RefineBlock((dither) != 0 ? dblock : block, &max16, &min16, (uint) (mask))) != 0)
					{
						if (max16 != min16)
						{
							stb__EvalColors(color, (ushort) (max16), (ushort) (min16));
							mask = (uint) (stb__MatchColorsBlock(block, color, (int) (dither)));
						}
						else
						{
							mask = (uint) (0);
							break;
						}
					}
					if ((mask) == (lastmask)) break;
				}
			}

			if ((max16) < (min16))
			{
				ushort t = (ushort) (min16);
				min16 = (ushort) (max16);
				max16 = (ushort) (t);
				mask ^= (uint) (0x55555555);
			}

			dest[0] = ((byte) (max16));
			dest[1] = ((byte) (max16 >> 8));
			dest[2] = ((byte) (min16));
			dest[3] = ((byte) (min16 >> 8));
			dest[4] = ((byte) (mask));
			dest[5] = ((byte) (mask >> 8));
			dest[6] = ((byte) (mask >> 16));
			dest[7] = ((byte) (mask >> 24));
		}

		public static void stb__CompressAlphaBlock(byte* dest, byte* src, int stride)
		{
			int i;
			int dist;
			int bias;
			int dist4;
			int dist2;
			int bits;
			int mask;
			int mn;
			int mx;
			mn = (int) (mx = (int) (src[0]));
			for (i = (int) (1); (i) < (16); i++)
			{
				if ((src[i*stride]) < (mn)) mn = (int) (src[i*stride]);
				else if ((src[i*stride]) > (mx)) mx = (int) (src[i*stride]);
			}
			(dest)[0] = (byte) (mx);
			(dest)[1] = (byte) (mn);
			dest += 2;
			dist = (int) (mx - mn);
			dist4 = (int) (dist*4);
			dist2 = (int) (dist*2);
			bias = (int) (((dist) < (8)) ? (dist - 1) : (dist/2 + 2));
			bias -= (int) (mn*7);
			bits = (int) (0);
			mask = (int) (0);
			for (i = (int) (0); (i) < (16); i++)
			{
				int a = (int) (src[i*stride]*7 + bias);
				int ind;
				int t;
				t = (int) (((a) >= (dist4)) ? -1 : 0);
				ind = (int) (t & 4);
				a -= (int) (dist4 & t);
				t = (int) (((a) >= (dist2)) ? -1 : 0);
				ind += (int) (t & 2);
				a -= (int) (dist2 & t);
				ind += (int) ((a) >= (dist) ? 1 : 0);
				ind = (int) (-ind & 7);
				ind ^= (int) ((2) > (ind) ? 1 : 0);
				mask |= (int) (ind << bits);
				if ((bits += (int) (3)) >= (8))
				{
					*dest++ = (byte) (mask);
					mask >>= 8;
					bits -= (int) (8);
				}
			}
		}

		public static void stb__InitDXT()
		{
			int i;
			for (i = (int) (0); (i) < (32); i++)
			{
				stb__Expand5[i] = (byte) ((i << 3) | (i >> 2));
			}
			for (i = (int) (0); (i) < (64); i++)
			{
				stb__Expand6[i] = (byte) ((i << 2) | (i >> 4));
			}
			for (i = (int) (0); (i) < (256 + 16); i++)
			{
				int v = (int) ((i - 8) < (0) ? 0 : (i - 8) > (255) ? 255 : i - 8);
				stb__QuantRBTab[i] = (byte) (stb__Expand5[stb__Mul8Bit((int) (v), (int) (31))]);
				stb__QuantGTab[i] = (byte) (stb__Expand6[stb__Mul8Bit((int) (v), (int) (63))]);
			}
			stb__PrepareOptTable(stb__OMatch5, stb__Expand5, (int) (32));
			stb__PrepareOptTable(stb__OMatch6, stb__Expand6, (int) (64));
		}

		public static void stb_compress_dxt_block(byte* dest, byte* src, int alpha, int mode)
		{
			if ((init) != 0)
			{
				stb__InitDXT();
				init = (int) (0);
			}

			if ((alpha) != 0)
			{
				stb__CompressAlphaBlock(dest, src + 3, (int) (4));
				dest += 8;
			}

			stb__CompressColorBlock(dest, src, (int) (mode));
		}

		public static void stb_compress_bc4_block(byte* dest, byte* src)
		{
			stb__CompressAlphaBlock(dest, src, (int) (1));
		}

		public static void stb_compress_bc5_block(byte* dest, byte* src)
		{
			stb__CompressAlphaBlock(dest, src, (int) (2));
			stb__CompressAlphaBlock(dest + 8, src + 1, (int) (2));
		}
	}
}