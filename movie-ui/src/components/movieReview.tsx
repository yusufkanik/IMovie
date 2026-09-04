'use client';

import { useState, useEffect } from 'react';
import { useAuth } from '@/context/AuthContext';
import { movieService } from '@/services/movieService';

// Backend ReviewResponseDto Karşılığı
export interface ReviewResponseDto {
  id: number;
  rating: number;
  comment: string;
  userEmail: string;
  createdAt: string;
  likeCount: number;
  dislikeCount: number;
}

// Backend CreateReviewDto Karşılığı
export interface CreateReviewDto {
  rating: number;
  comment: string;
}

interface MovieReviewsProps {
  movieId: number;
  onReviewSubmitted?: () => void;
}

export default function MovieReviews({movieId, onReviewSubmitted} : MovieReviewsProps) {

    // adding review
    const {user, isAuthenticated} = useAuth();
    const [reviews, setReviews] = useState<ReviewResponseDto[]>([]);
    const [rating, setRating] = useState<number>(10);
    const [comment, setComment] = useState('');

    // editing review
    const [editingReviewId, setEditingReviewId] = useState<number | null>(null);
    const [editRating, setEditRating] = useState<number>(10);
    const [editComment, setEditComment] = useState('');

    // submitting and loading
    const [loading, setLoading] = useState(true);
    const [submitting, setSubmitting] = useState(false);
    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const fetchReviews = async () => {

        try {
            setLoading(true);

            const data = await movieService.getMovieReviews(movieId);
            setReviews(data || []);
        }
        catch(err) {
            console.error("Yorumlar çekilirken hata oluştu", err);
        }
        finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        if (movieId) {
            fetchReviews();
        }
    }, [movieId]);

    const HandleSubmit = async (e: React.SubmitEvent) => {
        e.preventDefault();

        if (!isAuthenticated) return;

        setSubmitting(true);
        setError('');
        setSuccess('');

        try {
            const payload = {
                rating: Number(rating),
                comment: comment.trim()
            };

            await movieService.addReview(movieId, payload);
            setSuccess('Yorumunuz başarıyla eklendi!');
            setComment('');
            setRating(10);

            await fetchReviews();

            if (onReviewSubmitted) onReviewSubmitted();


        } catch (err: any) {
            console.error(err);
            const backendError = err.response?.data?.message || 'Yorum gönderilirken bir hata oluştu.';
            setError(backendError);

        } finally {
            setSubmitting(false);
        }

    };

    const HandleStartEdit = (rev: ReviewResponseDto) => {
        setEditingReviewId(rev.id);
        setEditComment(rev.comment);
        setEditRating(rev.rating);
    }

    const HandleUpdate = async (reviewId: number) => {
        try {
            await movieService.updateReview(movieId, reviewId, {rating: editRating, comment: editComment});
            setEditingReviewId(null);
            await fetchReviews();
            if (onReviewSubmitted) onReviewSubmitted();
        }
        catch (err) {
            console.error('Yorum güncellenemedi:', err);
        }
    };

    const handleDelete = async (reviewId: number) => {
        if (!confirm('Bu yorumu silmek istediğinize emin misiniz?')) return;

        try {
            await movieService.deleteReview(movieId, reviewId);
            await fetchReviews();
            if (onReviewSubmitted) onReviewSubmitted();
        } catch (err) {
            console.error('Yorum silinemedi:', err);
        }
    };

    const HandleLike = async (reviewId: number) => {
        if (!isAuthenticated) return;

        try {
            await movieService.likeReview(movieId, reviewId);
            await fetchReviews();
        }
        catch (err) {
            console.error('Beğeni işlemi başarısız:', err);
        }
    };

    const handleDislike = async (reviewId: number) => {
        if (!isAuthenticated) return;
        try {
            await movieService.dislikeReview(movieId, reviewId);
            await fetchReviews();
        } catch (err) {
            console.error('Beğenmeme işlemi başarısız:', err);
        }
    };

return (
    <section className="space-y-8 pt-8 border-t border-slate-800">
      <h2 className="text-xl font-bold text-slate-200">Değerlendirmeler & Yorumlar</h2>

      {/* Yorum Ekleme Formu */}
      {isAuthenticated ? (
        <form onSubmit={HandleSubmit} className="bg-slate-900 p-5 rounded-xl border border-slate-800 space-y-4">
          <h3 className="text-sm font-semibold text-slate-300">Filmi Değerlendir</h3>

          {error && <p className="text-xs text-rose-400 bg-rose-500/10 p-2.5 rounded-lg border border-rose-500/20">{error}</p>}
          {success && <p className="text-xs text-emerald-400 bg-emerald-500/10 p-2.5 rounded-lg border border-emerald-500/20">{success}</p>}

          <div className="flex items-center gap-4">
            <label className="text-xs text-slate-400">Puanınız (1-10):</label>
            <select
              value={rating}
              onChange={(e) => setRating(Number(e.target.value))}
              className="bg-slate-800 border border-slate-700 text-yellow-400 font-bold text-sm rounded-lg px-3 py-1.5 focus:outline-none cursor-pointer"
            >
              {[10, 9, 8, 7, 6, 5, 4, 3, 2, 1].map((num) => (
                <option key={num} value={num}>★ {num}</option>
              ))}
            </select>
          </div>

          <textarea
            value={comment}
            onChange={(e) => setComment(e.target.value)}
            placeholder="Film hakkındaki düşüncelerinizi yazın..."
            required
            maxLength={1000}
            rows={3}
            className="w-full bg-slate-800 border border-slate-700 rounded-lg p-3 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:border-indigo-500 resize-none"
          />

          <button
            type="submit"
            disabled={submitting || !comment.trim()}
            className="px-5 py-2 bg-indigo-600 hover:bg-indigo-500 disabled:opacity-50 text-white rounded-lg text-xs font-semibold transition-colors cursor-pointer"
          >
            {submitting ? 'Gönderiliyor...' : 'Yorum Yap'}
          </button>
        </form>
      ) : (
        <div className="bg-slate-900/50 p-4 rounded-xl border border-slate-800 text-center">
          <p className="text-xs text-slate-400">Yorum yapmak ve puan vermek için giriş yapmalısınız.</p>
        </div>
      )}

      {/* Yorum Listesi */}
      <div className="space-y-4">
        {loading ? (
          <p className="text-xs text-slate-500 animate-pulse">Yorumlar yükleniyor...</p>
        ) : reviews.length === 0 ? (
          <p className="text-xs text-slate-500 italic">Henüz yorum yapılmamış. İlk yorumu sen yap!</p>
        ) : (
          reviews.map((rev) => {
            const isOwner = user?.email === rev.userEmail;
            const isEditing = editingReviewId === rev.id;

            return (
              <div key={rev.id} className="bg-slate-900/60 p-4 rounded-xl border border-slate-800/80 space-y-3">
                <div className="flex items-center justify-between">
                  <div className="flex items-center gap-2">
                    <span className="text-xs font-bold text-slate-300">{rev.userEmail}</span>
                    <span className="text-[10px] text-slate-500">
                      {new Date(rev.createdAt).toLocaleDateString('tr-TR', { day: 'numeric', month: 'short', year: 'numeric' })}
                    </span>
                  </div>

                  <div className="flex items-center gap-3">
                    <span className="text-xs text-yellow-400 font-bold bg-yellow-500/10 px-2 py-0.5 rounded border border-yellow-500/20">
                      ★ {isEditing ? editRating : rev.rating}/10
                    </span>

                    {/* Sahibi İse Düzenle / Sil Butonları */}
                    {isOwner && !isEditing && (
                      <div className="flex items-center gap-2 text-xs">
                        <button
                          onClick={() => HandleStartEdit(rev)}
                          className="text-slate-400 hover:text-indigo-400 transition-colors cursor-pointer"
                        >
                          Düzenle
                        </button>
                        <button
                          onClick={() => handleDelete(rev.id)}
                          className="text-slate-400 hover:text-rose-400 transition-colors cursor-pointer"
                        >
                          Sil
                        </button>
                      </div>
                    )}
                  </div>
                </div>

                {/* Inline Düzenleme Modu */}
                {isEditing ? (
                  <div className="space-y-3 pt-2">
                    <div className="flex items-center gap-3">
                      <label className="text-xs text-slate-400">Yeni Puan:</label>
                      <select
                        value={editRating}
                        onChange={(e) => setEditRating(Number(e.target.value))}
                        className="bg-slate-800 border border-slate-700 text-yellow-400 text-xs rounded px-2 py-1"
                      >
                        {[10, 9, 8, 7, 6, 5, 4, 3, 2, 1].map((n) => (
                          <option key={n} value={n}>★ {n}</option>
                        ))}
                      </select>
                    </div>

                    <textarea
                      value={editComment}
                      onChange={(e) => setEditComment(e.target.value)}
                      rows={2}
                      className="w-full bg-slate-800 border border-slate-700 rounded-lg p-2.5 text-xs text-slate-100 focus:outline-none"
                    />

                    <div className="flex justify-end gap-2">
                      <button
                        onClick={() => setEditingReviewId(null)}
                        className="px-3 py-1 bg-slate-800 hover:bg-slate-700 text-slate-300 rounded text-xs cursor-pointer"
                      >
                        İptal
                      </button>
                      <button
                        onClick={() => HandleUpdate(rev.id)}
                        className="px-3 py-1 bg-indigo-600 hover:bg-indigo-500 text-white rounded text-xs cursor-pointer"
                      >
                        Kaydet
                      </button>
                    </div>
                  </div>
                ) : (
                  <p className="text-sm text-slate-300 leading-relaxed">{rev.comment}</p>
                )}

                {/* Beğeni / Beğenmeme Butonları */}
                <div className="flex items-center gap-3 text-xs text-slate-400 pt-1">
                  <button
                    onClick={() => HandleLike(rev.id)}
                    disabled={!isAuthenticated}
                    className="flex items-center gap-1 hover:text-emerald-400 transition-colors disabled:opacity-50 cursor-pointer"
                  >
                    👍 <span>{rev.likeCount}</span>
                  </button>
                  <button
                    onClick={() => handleDislike(rev.id)}
                    disabled={!isAuthenticated}
                    className="flex items-center gap-1 hover:text-rose-400 transition-colors disabled:opacity-50 cursor-pointer"
                  >
                    👎 <span>{rev.dislikeCount}</span>
                  </button>
                </div>
              </div>
            );
          })
        )}
      </div>
    </section>
  );
}