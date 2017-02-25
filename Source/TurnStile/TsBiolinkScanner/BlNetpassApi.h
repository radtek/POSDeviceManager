//[]------------------------------------------------------------------------[]
// BLNetPass API export declaration
//
//
//[]------------------------------------------------------------------------[]

#ifndef __BLNETPASSAPI_H
#define __BLNETPASSAPI_H

#ifdef	__BUILD_DLL
#define	BLNETPASS_EXPORT __declspec(dllexport)
#else
#define BLNETPASS_EXPORT
#endif

#include "BlNetpassUid.h"

extern "C"
{
    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassCreateA
				    (LPCSTR  pHostName, const GUID *, VOID **);

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassCreateW
				    (LPCWSTR pHostName, const GUID *, VOID **);

    BLNETPASS_EXPORT  HRESULT WINAPI CanBeUnload (DWORD dTimeout );

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassAccess_Open
				    (VOID ** rINetPassAccess, LPCSTR  pHostName	 ,
							const GUID *  cid	 ,
							      DWORD   dDevId	 );
    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassAccess_SetEvent 
				    (VOID *  pINetPassAccess, DWORD   dEvent	 );

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassAccess_SetEventEx 
				    (VOID *  pINetPassAccess, DWORD   dEvent	 ,
							      DWORD   dFlags	 ,
							      DWORD   dUserId	 ,
							const char *  pUserId	 ,
							const BYTE *  pWiegandId );

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassAccess_WriteDeviceReg
				    (VOID *  pINetPassAccess, DWORD   dOffset	 ,
							      void  * pBuf	 ,
							      DWORD   dBufSize	 ,
							      DWORD * pTransfered);

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassAccess_ReadDeviceReg
				    (VOID *  pINetPassAccess, DWORD   dOffset	 ,
							      void  * pBuf	 ,
							      DWORD   dBufSize	 ,
							      DWORD * pTransfered);

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassAccess_Close (VOID * pINetPassAccess);
//
//[]------------------------------------------------------------------------[]
//
    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassUserDb_Open
				    (VOID ** rIUserDb, LPCSTR  pHostName,
						 const GUID *  cid	,
						       DWORD   dDevId	);

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassUserDb_CreateUser 
				    (VOID * pIUserDb, DWORD   dUserId);

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassUserDb_DeleteUser 
				    (VOID * pIUserDb, DWORD   dUserId);

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassUserDb_GetValue
				    (VOID * pIUserDb, DWORD   dUserId	  ,
						      DWORD   dValueKey	  ,
						      VOID  * pBuf	  ,
						      DWORD   dBufSize    ,
						      DWORD * pTransfered );

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassUserDb_SetValue
				    (VOID * pIUserDb, DWORD   dUserId	  ,
						      DWORD   dValueKey	  ,
						      VOID  * pBuf	  ,
						      DWORD   dBufSize    ,
						      DWORD * pTransfered );

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassUserDb_Close (VOID * pIUserDb);

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassUserDb_ReadEventLog
				    (VOID * pIUserDb, DWORD   dRecId	  ,
						      VOID  * pBuf	  ,
						      DWORD   dBufSize	  ,
						      DWORD * pTransfered );

    BLNETPASS_EXPORT  HRESULT WINAPI BlNetPassUserDb_EraseEventLog
				    (VOID * pIUserDb, DWORD   dRecId	  );
}

//[]------------------------------------------------------------------------[]
//
//
#ifdef	UNICODE
#define	BlNetPassCreate	BlNetPassCreateW
#else
#define BlNetPassCreate	BlNetPassCreateA
#endif

#undef  INTERFACE
#define INTERFACE   INetPass
DECLARE_INTERFACE_ (INetPass, IUnknown)
{
// IUnknown methods

    STDMETHOD	(	 QueryInterface)(REFIID, VOID **)	PURE;
    STDMETHOD_	(ULONG,		 AddRef)()			PURE;
    STDMETHOD_	(ULONG,		Release)()			PURE;

// INetPass methods

    STDMETHOD	(     CreateNetPassEnum)(VOID **, REFIID cid,
						  REFIID iid)	PURE;
};

#define BLNETPASS_ADDRESS_MAX	32

#pragma pack (1)
struct BlNetPassDeviceInfo
{
    DWORD	dDeviceId   ;
    char	pAddress [BLNETPASS_ADDRESS_MAX];
};

struct BlNetPassInfo
{
    int		cEntries    ;
    BlNetPassDeviceInfo
		pEntries [1];
};
#pragma pack ()

#undef  INTERFACE
#define INTERFACE
DECLARE_INTERFACE_ (INetPassEnum, IUnknown)
{
// IUnknown methods

    STDMETHOD	(	 QueryInterface)(REFIID, VOID **)		PURE;
    STDMETHOD_	(ULONG,		 AddRef)()				PURE;
    STDMETHOD_	(ULONG,		Release)()				PURE;

// INetPassEnum methods

    STDMETHOD	(	 GetDevicesInfo)(VOID *	  pBlNetPassInfo     ,
					 DWORD	  dBlNetPassInfoSize )	PURE;

    STDMETHOD	(	 RegisterDevice)(	  DWORD	 dDevId,
					    const char * pAddr )	PURE;

    STDMETHOD	(      UnRegisterDevice)(	  DWORD	 dDevId)	PURE;

    STDMETHOD	(	   CreateDevice)(VOID **, REFIID iid   , 
						  DWORD	 dDevId)	PURE;
};

#undef  INTERFACE
#define INTERFACE
DECLARE_INTERFACE_ (INetPassScanner, IUnknown)
{
// IUnknown methods

    STDMETHOD	(	 QueryInterface)(REFIID, VOID **)		PURE;
    STDMETHOD_	(ULONG,		 AddRef)()				PURE;
    STDMETHOD_	(ULONG,		Release)()				PURE;

// INetPassScanner methods

    STDMETHOD	(	   GetImageInfo)(LONG  * pSizeX	       ,
					 LONG  * pSizeY	       ,
					 DWORD * pResX	       ,
					 DWORD * pResY	       ,
					 DWORD * pFlags	       )	PURE;

    STDMETHOD 	(	       GetImage)(void *  pImageBuf     ,
					 DWORD   dImageBufSize ,
					 DWORD * pImageSize    ,
					 DWORD	 dTimeout      )	PURE;
};

typedef void (WINAPI * BlNetPassCallBack)(DWORD Result, DWORD  dTransfered ,
							void * pContext    );

#undef  INTERFACE
#define INTERFACE
DECLARE_INTERFACE_ (INetPassScanner2, INetPassScanner)
{

// INetPassScanner2 methods

    STDMETHOD 	(	     GetImageEx)(void  * pImageBuf     ,
					 DWORD   dImageBufSize ,
					 DWORD	 dTimeout      ,
			     BlNetPassCallBack	 pCProc	       ,
					 void  * pCContext     )	PURE;
};

#undef  INTERFACE
#define INTERFACE
DECLARE_INTERFACE_ (INetPassAccess, IUnknown)
{
// IUnknown methods

    STDMETHOD	(	 QueryInterface)(REFIID, VOID **)		PURE;
    STDMETHOD_	(ULONG,		 AddRef)()				PURE;
    STDMETHOD_	(ULONG,		Release)()				PURE;

// INetPassAccess methods

    STDMETHOD	(	       SetEvent)(DWORD dEventCode)		PURE;

    STDMETHOD	(	 WriteDeviceReg)(DWORD dOffset, void *  pBuf	   ,
							DWORD	dBufSize   ,
							DWORD * pTransfered) PURE;

    STDMETHOD	(	  ReadDeviceReg)(DWORD dOffset, void  * pBuf	   , 
							DWORD   dBufSize   , 
							DWORD * pTransfered) PURE;
};

#undef  INTERFACE
#define INTERFACE
DECLARE_INTERFACE_ (INetPassAccess2, IUnknown)
{
// IUnknown methods

    STDMETHOD	(	 QueryInterface)(REFIID, VOID **)		PURE;
    STDMETHOD_	(ULONG,		 AddRef)()				PURE;
    STDMETHOD_	(ULONG,		Release)()				PURE;

// INetPassAccess methods

    STDMETHOD	(	       SetEvent)(DWORD dEventCode)		PURE;

    STDMETHOD	(	 WriteDeviceReg)(DWORD dOffset, void *  pBuf	   ,
							DWORD	dBufSize   ,
							DWORD * pTransfered) PURE;

    STDMETHOD	(	  ReadDeviceReg)(DWORD dOffset, void  * pBuf	   , 
							DWORD   dBufSize   , 
							DWORD * pTransfered) PURE;

    STDMETHOD	(	     SetEventEx)(DWORD dEventCode ,
					 DWORD dEventFlags,
					 DWORD dUserId	  , const char * pUserId ,
							    const BYTE * pWiegand) PURE;
};

#define	BLNETPASS_USERID_ENUM		0xFFFFFFFF

#define BLNETPASS_USERDB_KEY_FULL	0xFFFFFFFF
#define BLNETPASS_USERDB_KEY_HEAD	0xFFFFFFFE
#define BLNETPASS_USERDB_KEY_MASK	1
#define BLNETPASS_USERDB_KEY_USERDCR	1
#define	BLNETPASS_USERDB_KEY_TEMPLATE	2

#pragma pack (push, 1)
struct BlNetPassUserDcr
{
    BYTE	bSize	;			// Full structure size
    BYTE	bCode	;			// 0xxxxxxx - Bigendian raw data
						// 1xxxxxxx - String data
//  BYTE	bData [];
};

struct BlNetPassUserDcrHeader
{
    DWORD	dSize	;
};

struct BlNetPassUserDcrRec
{
    BlNetPassUserDcrHeader	Header ;
    BlNetPassUserDcr		Dcr [1];
};

#define	USERDCR_CODE_ID_GUID	    0x01	// Biotime's user GUID (Any format)
#define USERDCR_CODE_ID_PINID	    0x02	// Pin Id  (Bigendian)
#define USERDCR_CODE_ID_CARD	    0x03	// Card Id (Bigendian)
#define USERDCR_CODE_ID_WEIGAND	    0x04	// Weigand control code (Bigendian)
#define USERDCR_CODE_ID_NAME	    0x81	// User Display name
//
//....................................
//
struct BlNetPassTemplate
{
    DWORD	dSize	    ;			// Full structure size
//  BYTE	bTemplate [];			// Template body
};

struct BlNetPassTemplateRecHeader
{
    DWORD	dSize	    ;			// Full structure size
};

struct BlNetPassTemplateRec
{
    BlNetPassTemplateRecHeader  Header	     ;
    BlNetPassTemplate		Templates [1];
};
//
//....................................
//
struct BlNetPassEventRecHeader
{
    DWORD	dSize	    ;			// Full structure size as version
};

struct BlNetPassDateTime
{
    DWORD   sec	    : 6;			// 0-59
    DWORD   min	    : 6;			// 0-59
    DWORD   hour    : 5;			// 0-23
    DWORD   day	    : 5;			// 1-31
    DWORD   month   : 4;			// 1-12
    DWORD   year    : 6;			// Year % 4
};

struct BlNetPassEventRec
{
    BlNetPassEventRecHeader	Header	    ;
    DWORD			dRecordId   ;
    BlNetPassDateTime		dDateTime   ;
    DWORD			dUserId	    ;
    DWORD			dEventId    ;
//  BYTE			bData []	// Event body
};
#pragma pack (pop)

#undef  INTERFACE
#define INTERFACE
DECLARE_INTERFACE_ (INetPassUserDb, IUnknown)
{
// IUnknown methods

    STDMETHOD	(	 QueryInterface)(REFIID, VOID **)		PURE;
    STDMETHOD_	(ULONG,		 AddRef)()				PURE;
    STDMETHOD_	(ULONG,		Release)()				PURE;

// INetPassUserDb methods

    STDMETHOD	(   	     CreateUser)(DWORD dUserId)			PURE;

    STDMETHOD	(	     DeleteUser)(DWORD dUserId)			PURE;

    STDMETHOD	(	   GetUserValue)(DWORD dUserId, DWORD   dValueKey  ,
							void  *	pBuf	   ,
							DWORD	dBufSize   ,
							DWORD *	dTransfered) PURE;

    STDMETHOD	(	   SetUserValue)(DWORD dUserId, DWORD	dValueKey  ,
							void  * pBuf	   ,
							DWORD	dBufSize   ,
							DWORD *	pTransfered) PURE;

    STDMETHOD	(	   ReadEventLog)(DWORD dRecId,	void  * pBuf	   ,
							DWORD	dBufSize   ,
							DWORD *	pTransfered) PURE;

    STDMETHOD	(	  EraseEventLog)(DWORD dRecId)			     PURE;
};
//
//[]------------------------------------------------------------------------[]

#endif // __BLNETPASSAPI_H
