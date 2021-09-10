#include <random>
#include <iostream>
#include "Secret.h"

std::random_device rd;
std::uniform_int_distribution dist(0, 100);

int init()
{
	std::cout << "Hi, I'm a dummy initializer" << std::endl;
	return dist(rd);
}

int secret = init();

extern "C"
{
	__declspec(dllexport) __cdecl int GetSecret()
	{
		return secret;
	}
}