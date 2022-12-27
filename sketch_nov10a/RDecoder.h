


struct ENCODED_MSG {
    unsigned char* encoded_data;
    unsigned short size;
};

struct Package {
    //DateTime Timestamp;
    unsigned char Addr;
    int Size;
    unsigned char CrcHeader;
    unsigned char* Data;
    int data_size;
    unsigned char CrcOverall;
};

void decode(uint8_t *data, int offset, int size, void (*OnPackageReceived)(Package));

ENCODED_MSG encode(uint8_t addr, uint8_t data[], uint16_t count);
