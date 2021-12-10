// BloomHeader.cpp: implementation of the CBloomHeader class.
//
//////////////////////////////////////////////////////////////////////

#include "pch.h"
#include "BloomHeader.h"

#ifdef _DEBUG
#undef THIS_FILE
static char THIS_FILE[] = __FILE__;
#endif

//////////////////////////////////////////////////////////////////////
// Construction/Destruction
//////////////////////////////////////////////////////////////////////

CBloomHeader::CBloomHeader()
{
	memset(&Head, 0, sizeof Head);
}

void CBloomHeader::Default(CHead& Head)
{
	memset(&Head, 0, sizeof(Head));
	Head.head[0] = 'b';
	Head.head[1] = 'l';
	Head.head[2] = 'o';
	Head.head[3] = 'o';
	Head.head[4] = 'm';
}

void CBloomHeader::Default()
{
	Default(Head);
}

int CBloomHeader::Check() const
{
	int res;
	CHead h;
	Default(h);
	res = memcmp(Head.head, h.head, sizeof Head.head);
	if (res == 0)
		return BLOOM_VERSION;
	return 0;
}

void CBloomHeader::Load(int file)
{
	IO_Validate(_lseek(file, 0, SEEK_SET));
	IO_Validate(_read(file, &Head, sizeof Head));
}

void CBloomHeader::Save(int file)
{
	IO_Validate(_lseek(file, 0, SEEK_SET));
	IO_Validate(_chsize(file, 0));
	IO_Validate(_write(file, &Head, sizeof Head));
}
