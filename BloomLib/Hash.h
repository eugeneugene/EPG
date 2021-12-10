#pragma once

class CHash
{
private:
	std::unique_ptr<unsigned> HashVal;
	UINT size;
	void Reset(unsigned nNewSize) { HashVal.reset(new unsigned[nNewSize]); }
public:
	CHash(unsigned s = BLOOM_DEFAULT_HASHSIZE) { Reset(size = s); }
	unsigned Size() const { return size; }
	void Size(unsigned s) { Reset(size = s); }
	unsigned* GetHash() const { return HashVal.get(); }
	unsigned& operator[](std::ptrdiff_t i) const { return GetHash()[i]; }
};
