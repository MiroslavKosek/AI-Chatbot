  import { toast } from 'react-toastify';

export function toastifySuccess(text: string){
    toast.success(`✅ ${text}`, {
    position: "bottom-left",
    autoClose: 3500,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
    draggable: false,
    progress: undefined,
    theme: "light",
    });
}

export function toastifyError(text: string){
    toast.error(`❗️${text}`, {
    position: "bottom-left",
    autoClose: 3500,
    hideProgressBar: false,
    closeOnClick: true,
    pauseOnHover: true,
    draggable: false,
    progress: undefined,
    theme: "light",
    });
}