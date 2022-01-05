#include <iostream>

thread_local int secret = 69;

extern "C"
{
	int GetThreadLocalInt()
	{
		std::cout << "Address of secret: " << &secret << std::endl;
		return secret++;
	}
}
