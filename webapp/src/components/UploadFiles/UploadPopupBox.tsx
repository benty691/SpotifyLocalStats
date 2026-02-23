function UploadPopupBox({ uploadResult }: { uploadResult: string }) {
  return (
    <>
      <div className='block justify-center align-middle  min-h-screen min-w-screen z-50'>
        <div className='flex-col justify-center align-middle self-center border border-accent-cyan'>
          <h2 className='text-center'>{uploadResult}</h2>
          <button className='text-center'>OK</button>
        </div>
      </div>
    </>
  );
}

export default UploadPopupBox;
