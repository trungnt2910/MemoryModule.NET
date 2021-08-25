#include <random>
#include "Secret.h"

std::random_device rd;
std::uniform_int_distribution dist(0, 100);

extern "C"
{
	__declspec(dllexport) __cdecl int GetSecret()
	{
		return dist(rd);
	}
}