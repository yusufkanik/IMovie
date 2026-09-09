'use client';

import React, { useEffect, useState } from 'react';
import Link from 'next/link';
import { customListService, CustomListSummary } from '@/services/CustomListService';

export default function PublicListsPage() {

    const [lists, setLists] = useState<CustomListSummary[]>([]);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        
        const fetchPublicLists = async () => {
            try {
                setLoading(true);
                const res = await customListService.getPublicLists();
                setLists(res.data);
            }
            catch (err) {
                console.error("Listeler çekilemedi.", err);
                setLists([]);
            }
            finally {
                setLoading(false);
            }
        }

        fetchPublicLists();
        
    }, [])

    return (
    <div className="min-h-screen bg-slate-950 text-white p-6 md:p-12">
      <div className="max-w-5xl mx-auto space-y-8">
        <div>
          <h1 className="text-3xl font-extrabold text-slate-100">Topluluk Listeleri</h1>
          <p className="text-slate-400 text-sm mt-1">
            Diğer kullanıcılar tarafından oluşturulan öne çıkan film koleksiyonlarını keşfedin.
          </p>
        </div>

        {loading ? (
          <p className="text-slate-500 text-sm">Listeler yükleniyor...</p>
        ) : lists.length === 0 ? (
          <p className="text-slate-500 text-sm">Henüz herkese açık paylaşılan bir liste yok.</p>
        ) : (
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
            {lists.map((list) => (
              <Link
                key={list.id}
                href={`/lists/${list.id}`}
                className="bg-slate-900 border border-slate-800 p-5 rounded-xl hover:border-indigo-500/50 transition block"
              >
                <h3 className="font-bold text-base text-slate-100">{list.title}</h3>
                {list.description && (
                  <p className="text-xs text-slate-400 mt-1 line-clamp-2">{list.description}</p>
                )}
                <div className="mt-4 text-[11px] text-slate-500 flex items-center justify-between pt-2 border-t border-slate-800">
                  <span className="text-indigo-400 font-medium">{list.movieCount} Film</span>
                  <span>{new Date(list.createdAt).toLocaleDateString()}</span>
                </div>
              </Link>
            ))}
          </div>
        )}
      </div>
    </div>
  );

}