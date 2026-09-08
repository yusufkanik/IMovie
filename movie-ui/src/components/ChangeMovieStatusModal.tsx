'use client';

import { useState } from 'react';
import { userService, WatchStatus } from '@/services/userService';

interface AddToListModalProps {
  movieId: number;
  initialStatus?: WatchStatus | null; // Mevcut durum varsa gelecek
  onClose: () => void;
  onSuccess?: (newStatus: WatchStatus) => void; // Seçilen yeni durumu sayfaya ileteceğiz
}

export function AddToListModal({ movieId, initialStatus, onClose, onSuccess }: AddToListModalProps) {
  const [loading, setLoading] = useState(false);
  
  // Varsayılan olarak mevcut durum varsa onu, yoksa PlanToWatch seçiyoruz
  const [selectedStatus, setSelectedStatus] = useState<WatchStatus>(
    initialStatus || WatchStatus.PlanToWatch
  );

  const handleSave = async () => {
    setLoading(true);
    try {
      await userService.setWatchStatus(movieId, selectedStatus);
      if (onSuccess) onSuccess(selectedStatus); // Sayfaya güncel durumu bildir
      onClose();
    } catch (err) {
      console.error('Film durumu güncellenemedi:', err);
      alert('İşlem başarısız oldu. Lütfen tekrar deneyin.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/70 backdrop-blur-sm p-4">
      <div className="bg-slate-900 border border-slate-800 rounded-2xl p-6 w-full max-w-md space-y-6 shadow-2xl animate-in fade-in zoom-in-95 duration-200">
        <div className="flex justify-between items-center border-b border-slate-800 pb-4">
          <h2 className="text-lg font-bold text-white">İzleme Durumunu Güncelle</h2>
          <button 
            onClick={onClose} 
            className="text-slate-400 hover:text-white transition cursor-pointer p-1 rounded-lg hover:bg-slate-800"
          >
            ✕
          </button>
        </div>

        <div className="space-y-3">
          <label className="text-xs text-slate-400 font-medium">Bir durum seçin:</label>
          <div className="grid grid-cols-1 gap-2.5">
            <button
              type="button"
              onClick={() => setSelectedStatus(WatchStatus.Watched)}
              className={`p-3.5 rounded-xl border text-left text-sm font-semibold transition-all cursor-pointer ${
                selectedStatus === WatchStatus.Watched
                  ? 'bg-emerald-600/20 border-emerald-500 text-emerald-300'
                  : 'bg-slate-800/50 border-slate-700/60 text-slate-300 hover:bg-slate-800'
              }`}
            >
              ✓ İzledim (Watched)
            </button>

            <button
              type="button"
              onClick={() => setSelectedStatus(WatchStatus.PlanToWatch)}
              className={`p-3.5 rounded-xl border text-left text-sm font-semibold transition-all cursor-pointer ${
                selectedStatus === WatchStatus.PlanToWatch
                  ? 'bg-amber-600/20 border-amber-500 text-amber-300'
                  : 'bg-slate-800/50 border-slate-700/60 text-slate-300 hover:bg-slate-800'
              }`}
            >
              📌 İzleyeceğim (Plan to Watch)
            </button>

            <button
              type="button"
              onClick={() => setSelectedStatus(WatchStatus.Dropped)}
              className={`p-3.5 rounded-xl border text-left text-sm font-semibold transition-all cursor-pointer ${
                selectedStatus === WatchStatus.Dropped
                  ? 'bg-rose-600/20 border-rose-500 text-rose-300'
                  : 'bg-slate-800/50 border-slate-700/60 text-slate-300 hover:bg-slate-800'
              }`}
            >
              ✕ Yarıda Bıraktım (Dropped)
            </button>
          </div>
        </div>

        <div className="flex gap-3 justify-end pt-2 border-t border-slate-800">
          <button
            onClick={onClose}
            className="px-4 py-2 rounded-xl bg-slate-800 text-slate-300 text-xs font-semibold hover:bg-slate-700 transition cursor-pointer"
          >
            İptal
          </button>
          <button
            onClick={handleSave}
            disabled={loading}
            className="px-5 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold disabled:opacity-50 transition cursor-pointer"
          >
            {loading ? 'Kaydediliyor...' : 'Kaydet'}
          </button>
        </div>
      </div>
    </div>
  );
}