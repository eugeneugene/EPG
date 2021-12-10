/* Copyright (C) 1995-1997 Eric Young (eay@cryptsoft.com)
 * All rights reserved.
 *
 * This package is an SSL implementation written
 * by Eric Young (eay@cryptsoft.com).
 * The implementation was written so as to conform with Netscapes SSL.
 *
 * This library is free for commercial and non-commercial use as long as
 * the following conditions are aheared to.  The following conditions
 * apply to all code found in this distribution, be it the RC4, RSA,
 * lhash, DES, etc., code; not just the SSL code.  The SSL documentation
 * included with this distribution is covered by the same copyright terms
 * except that the holder is Tim Hudson (tjh@cryptsoft.com).
 *
 * Copyright remains Eric Young's, and as such any Copyright notices in
 * the code are not to be removed.
 * If this package is used in a product, Eric Young should be given attribution
 * as the author of the parts of the library used.
 * This can be in the form of a textual message at program startup or
 * in documentation (online or textual) provided with the package.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions
 * are met:
 * 1. Redistributions of source code must retain the copyright
 *    notice, this list of conditions and the following disclaimer.
 * 2. Redistributions in binary form must reproduce the above copyright
 *    notice, this list of conditions and the following disclaimer in the
 *    documentation and/or other materials provided with the distribution.
 * 3. All advertising materials mentioning features or use of this software
 *    must display the following acknowledgement:
 *    "This product includes cryptographic software written by
 *     Eric Young (eay@cryptsoft.com)"
 *    The word 'cryptographic' can be left out if the rouines from the library
 *    being used are not cryptographic related :-).
 * 4. If you include any Windows specific code (or a derivative thereof) from
 *    the apps directory (application code) you must include an acknowledgement:
 *    "This product includes software written by Tim Hudson (tjh@cryptsoft.com)"
 *
 * THIS SOFTWARE IS PROVIDED BY ERIC YOUNG ``AS IS'' AND
 * ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
 * IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
 * ARE DISCLAIMED.  IN NO EVENT SHALL THE AUTHOR OR CONTRIBUTORS BE LIABLE
 * FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
 * DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS
 * OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION)
 * HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
 * LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY
 * OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF
 * SUCH DAMAGE.
 *
 * The licence and distribution terms for any publically available version or
 * derivative of this code cannot be changed.  i.e. this code cannot simply be
 * copied and put under another distribution licence
 * [including the GNU Public Licence.]
 */

#pragma once

#define DES_PTR
#define DES_RISC1
#define DES_UNROLL

#ifndef DES_LONG
#define DES_LONG unsigned long
#endif

#define DES_ENCRYPT     1
#define DES_DECRYPT     0

#define NUM_WEAK_KEY    16
#define ITERATIONS 16
#define HALF_ITERATIONS 8

typedef unsigned char des_cblock[8];

typedef struct des_ks_struct
{
	union {
		des_cblock _;
		/* make sure things are correct size on machines with
		* 8 byte longs */
		DES_LONG pad[2];
	} ks;
#undef _
#define _	ks._
} des_key_schedule[16];
#define DES_KEY_SZ      (sizeof(des_cblock))
#define DES_SCHEDULE_SZ (sizeof(des_key_schedule))

#define c2l(c,l)	(l =((DES_LONG)(*((c)++)))    , \
	l|=((DES_LONG)(*((c)++)))<< 8L, \
	l|=((DES_LONG)(*((c)++)))<<16L, \
	l|=((DES_LONG)(*((c)++)))<<24L)

#define l2c(l,c)	(*((c)++)=(unsigned char)(((l)     )&0xff), \
	*((c)++)=(unsigned char)(((l)>> 8L)&0xff), \
	*((c)++)=(unsigned char)(((l)>>16L)&0xff), \
	*((c)++)=(unsigned char)(((l)>>24L)&0xff))

#define PERM_OP(a,b,t,n,m) ((t)=((((a)>>(n))^(b))&(m)),\
	(b)^=(t),\
	(a)^=((t)<<(n)))

#define HPERM_OP(a,t,n,m) ((t)=((((a)<<(16-(n)))^(a))&(m)),\
	(a)=(a)^(t)^(t>>(16-(n))))

#define IP(l,r) \
{ \
	register DES_LONG tt; \
	PERM_OP(r,l,tt, 4,0x0f0f0f0fL); \
	PERM_OP(l,r,tt,16,0x0000ffffL); \
	PERM_OP(r,l,tt, 2,0x33333333L); \
	PERM_OP(l,r,tt, 8,0x00ff00ffL); \
	PERM_OP(r,l,tt, 1,0x55555555L); \
}

#define FP(l,r) \
{ \
	register DES_LONG tt; \
	PERM_OP(l,r,tt, 1,0x55555555L); \
	PERM_OP(r,l,tt, 8,0x00ff00ffL); \
	PERM_OP(l,r,tt, 2,0x33333333L); \
	PERM_OP(r,l,tt,16,0x0000ffffL); \
	PERM_OP(l,r,tt, 4,0x0f0f0f0fL); \
}

#define ROTATE(a,n)     (_lrotr(a,n))

/* Don't worry about the LOAD_DATA() stuff, that is used by
* fcrypt() to add it's little bit to the front */

#ifdef DES_FCRYPT

#define LOAD_DATA_tmp(R,S,u,t,E0,E1) \
{ DES_LONG tmp; LOAD_DATA(R,S,u,t,E0,E1,tmp); }

#define LOAD_DATA(R,S,u,t,E0,E1,tmp) \
	t=R^(R>>16L); \
	u=t&E0; t&=E1; \
	tmp=(u<<16); u^=R^s[S  ]; u^=tmp; \
	tmp=(t<<16); t^=R^s[S+1]; t^=tmp
#else
#define LOAD_DATA_tmp(a,b,c,d,e,f) LOAD_DATA(a,b,c,d,e,f,g)
#define LOAD_DATA(R,S,u,t,E0,E1,tmp) \
	u=R^s[S  ]; \
	t=R^s[S+1]
#endif

/* The changes to this macro may help or hinder, depending on the
* compiler and the architecture.  gcc2 always seems to do well :-).
* Inspired by Dana How <how@isl.stanford.edu>
* DO NOT use the alternative version on machines with 8 byte longs.
* It does not seem to work on the Alpha, even when DES_LONG is 4
* bytes, probably an issue of accessing non-word aligned objects :-( */
#ifdef DES_PTR

/* It recently occurred to me that 0^0^0^0^0^0^0 == 0, so there
* is no reason to not xor all the sub items together.  This potentially
* saves a register since things can be xored directly into L */

#if defined(DES_RISC1) || defined(DES_RISC2)
#ifdef DES_RISC1
#define D_ENCRYPT(LL,R,S) { \
	unsigned int u1,u2,u3; \
	LOAD_DATA(R,S,u,t,E0,E1,u1); \
	u2=(int)u>>8L; \
	u1=(int)u&0xfc; \
	u2&=0xfc; \
	t=ROTATE(t,4); \
	u>>=16L; \
	LL^= *(DES_LONG *)((unsigned char *)des_SP      +u1); \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x200+u2); \
	u3=(int)(u>>8L); \
	u1=(int)u&0xfc; \
	u3&=0xfc; \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x400+u1); \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x600+u3); \
	u2=(int)t>>8L; \
	u1=(int)t&0xfc; \
	u2&=0xfc; \
	t>>=16L; \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x100+u1); \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x300+u2); \
	u3=(int)t>>8L; \
	u1=(int)t&0xfc; \
	u3&=0xfc; \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x500+u1); \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x700+u3); }
#endif
#ifdef DES_RISC2
#define D_ENCRYPT(LL,R,S) { \
	unsigned int u1,u2,s1,s2; \
	LOAD_DATA(R,S,u,t,E0,E1,u1); \
	u2=(int)u>>8L; \
	u1=(int)u&0xfc; \
	u2&=0xfc; \
	t=ROTATE(t,4); \
	LL^= *(DES_LONG *)((unsigned char *)des_SP      +u1); \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x200+u2); \
	s1=(int)(u>>16L); \
	s2=(int)(u>>24L); \
	s1&=0xfc; \
	s2&=0xfc; \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x400+s1); \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x600+s2); \
	u2=(int)t>>8L; \
	u1=(int)t&0xfc; \
	u2&=0xfc; \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x100+u1); \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x300+u2); \
	s1=(int)(t>>16L); \
	s2=(int)(t>>24L); \
	s1&=0xfc; \
	s2&=0xfc; \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x500+s1); \
	LL^= *(DES_LONG *)((unsigned char *)des_SP+0x700+s2); }
#endif
#else
#define D_ENCRYPT(LL,R,S) { \
	LOAD_DATA_tmp(R,S,u,t,E0,E1); \
	t=ROTATE(t,4); \
	LL^= \
	*(DES_LONG *)((unsigned char *)des_SP      +((u     )&0xfc))^ \
	*(DES_LONG *)((unsigned char *)des_SP+0x200+((u>> 8L)&0xfc))^ \
	*(DES_LONG *)((unsigned char *)des_SP+0x400+((u>>16L)&0xfc))^ \
	*(DES_LONG *)((unsigned char *)des_SP+0x600+((u>>24L)&0xfc))^ \
	*(DES_LONG *)((unsigned char *)des_SP+0x100+((t     )&0xfc))^ \
	*(DES_LONG *)((unsigned char *)des_SP+0x300+((t>> 8L)&0xfc))^ \
	*(DES_LONG *)((unsigned char *)des_SP+0x500+((t>>16L)&0xfc))^ \
	*(DES_LONG *)((unsigned char *)des_SP+0x700+((t>>24L)&0xfc)); }
#endif

#else /* original version */

#if defined(DES_RISC1) || defined(DES_RISC2)
#ifdef DES_RISC1
#define D_ENCRYPT(LL,R,S) {\
	unsigned int u1,u2,u3; \
	LOAD_DATA(R,S,u,t,E0,E1,u1); \
	u>>=2L; \
	t=ROTATE(t,6); \
	u2=(int)u>>8L; \
	u1=(int)u&0x3f; \
	u2&=0x3f; \
	u>>=16L; \
	LL^=des_SPtrans[0][u1]; \
	LL^=des_SPtrans[2][u2]; \
	u3=(int)u>>8L; \
	u1=(int)u&0x3f; \
	u3&=0x3f; \
	LL^=des_SPtrans[4][u1]; \
	LL^=des_SPtrans[6][u3]; \
	u2=(int)t>>8L; \
	u1=(int)t&0x3f; \
	u2&=0x3f; \
	t>>=16L; \
	LL^=des_SPtrans[1][u1]; \
	LL^=des_SPtrans[3][u2]; \
	u3=(int)t>>8L; \
	u1=(int)t&0x3f; \
	u3&=0x3f; \
	LL^=des_SPtrans[5][u1]; \
	LL^=des_SPtrans[7][u3]; }
#endif
#ifdef DES_RISC2
#define D_ENCRYPT(LL,R,S) {\
	unsigned int u1,u2,s1,s2; \
	LOAD_DATA(R,S,u,t,E0,E1,u1); \
	u>>=2L; \
	t=ROTATE(t,6); \
	u2=(int)u>>8L; \
	u1=(int)u&0x3f; \
	u2&=0x3f; \
	LL^=des_SPtrans[0][u1]; \
	LL^=des_SPtrans[2][u2]; \
	s1=(int)u>>16L; \
	s2=(int)u>>24L; \
	s1&=0x3f; \
	s2&=0x3f; \
	LL^=des_SPtrans[4][s1]; \
	LL^=des_SPtrans[6][s2]; \
	u2=(int)t>>8L; \
	u1=(int)t&0x3f; \
	u2&=0x3f; \
	LL^=des_SPtrans[1][u1]; \
	LL^=des_SPtrans[3][u2]; \
	s1=(int)t>>16; \
	s2=(int)t>>24L; \
	s1&=0x3f; \
	s2&=0x3f; \
	LL^=des_SPtrans[5][s1]; \
	LL^=des_SPtrans[7][s2]; }
#endif

#else

#define D_ENCRYPT(LL,R,S) {\
	LOAD_DATA_tmp(R,S,u,t,E0,E1); \
	t=ROTATE(t,4); \
	LL^=\
	des_SPtrans[0][(u>> 2L)&0x3f]^ \
	des_SPtrans[2][(u>>10L)&0x3f]^ \
	des_SPtrans[4][(u>>18L)&0x3f]^ \
	des_SPtrans[6][(u>>26L)&0x3f]^ \
	des_SPtrans[1][(t>> 2L)&0x3f]^ \
	des_SPtrans[3][(t>>10L)&0x3f]^ \
	des_SPtrans[5][(t>>18L)&0x3f]^ \
	des_SPtrans[7][(t>>26L)&0x3f]; }
#endif
#endif

extern const DES_LONG des_SPtrans[8][64];
extern const DES_LONG des_skb[8][64];
extern const des_cblock weak_keys[NUM_WEAK_KEY];
extern const unsigned char odd_parity[256];

void des_ecb3_encrypt(des_cblock* input, des_cblock* output,
	des_key_schedule ks1, des_key_schedule ks2, des_key_schedule ks3, int enc);

void des_encrypt3(DES_LONG* data, des_key_schedule ks1,
	des_key_schedule ks2, des_key_schedule ks3);
void des_decrypt3(DES_LONG* data, des_key_schedule ks1,
	des_key_schedule ks2, des_key_schedule ks3);
void des_encrypt2(DES_LONG* data, des_key_schedule ks, int enc);
int des_set_key(des_cblock* key, des_key_schedule schedule);
