'use client';

import { useState } from 'react';
import Link from 'next/link';
import { useRouter } from 'next/navigation';
import { api } from '@/lib/api';

export default function RegisterPage() {

    const router = useRouter();

    const [loading, setLoading] = useState(false);
    const [formData, setFormData] = useState({
        username: '',
        email: '',
        password: '',
        confirmPassword: ''
    });

    const [error, setError] = useState('');
    const [success, setSuccess] = useState('');

    const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
    };
    
    const handleSubmit = async (e: React.SubmitEvent) => {

        e.preventDefault();
        setError('');
        setSuccess('');
        
        if (formData.password !== formData.confirmPassword) {
            setError("Şifreler birbiriyle eşleşmiyor.");
            return;
        }

        if (formData.password.length < 6) {
            setError("Şifre en az 6 karakter uzunluğunda olmalıdır.");
            return;
        }

        setLoading(true);

        try {
            await api.post(`auth/register`, {
                username: formData.username.trim(),
                email: formData.email.trim(),
                password: formData.password
            });

            setSuccess("Hesabınız başarıyla oluşturuldu.");

            setTimeout(() => {
                router.push(`/login`)
            }, 2000);
        }
        catch(err: any) {
            setError(err.response?.data?.message ||
                'Kayıt olurken bir hata oluştu. Lütfen bilgilerinizi kontrol edin.'
            );
        }
        finally {
            setLoading(false);
        }

    };

    return (
    <div className="min-h-screen bg-slate-950 text-white flex items-center justify-center p-4">
      <div className="w-full max-w-md bg-slate-900 border border-slate-800 rounded-2xl p-8 shadow-2xl space-y-6">
        
        {/* Başlık */}
        <div className="text-center space-y-2">
          <h1 className="text-2xl font-bold text-indigo-400">Aramıza Katıl</h1>
          <p className="text-xs text-slate-400">
            Film SaaS platformuna kayıt olarak kişiselleştirilmiş önerilerin tadını çıkarın.
          </p>
        </div>

        {/* Uyarı Mesajları */}
        {error && (
          <div className="bg-rose-500/10 border border-rose-500/20 text-rose-400 p-3 rounded-xl text-xs">
            {error}
          </div>
        )}

        {success && (
          <div className="bg-emerald-500/10 border border-emerald-500/20 text-emerald-400 p-3 rounded-xl text-xs">
            {success}
          </div>
        )}

        {/* Form */}
        <form onSubmit={handleSubmit} className="space-y-4">
          <div>
            <label className="block text-xs font-semibold text-slate-300 mb-1">
              Kullanıcı Adı
            </label>
            <input
              type="text"
              name="username"
              required
              value={formData.username}
              onChange={handleChange}
              placeholder="kullaniciadi"
              className="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:border-indigo-500"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-slate-300 mb-1">
              E-Posta
            </label>
            <input
              type="email"
              name="email"
              required
              value={formData.email}
              onChange={handleChange}
              placeholder="ornek@email.com"
              className="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:border-indigo-500"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-slate-300 mb-1">
              Şifre
            </label>
            <input
              type="password"
              name="password"
              required
              value={formData.password}
              onChange={handleChange}
              placeholder="••••••••"
              className="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:border-indigo-500"
            />
          </div>

          <div>
            <label className="block text-xs font-semibold text-slate-300 mb-1">
              Şifre Tekrarı
            </label>
            <input
              type="password"
              name="confirmPassword"
              required
              value={formData.confirmPassword}
              onChange={handleChange}
              placeholder="••••••••"
              className="w-full bg-slate-800 border border-slate-700 rounded-lg px-3 py-2 text-sm text-slate-100 placeholder-slate-500 focus:outline-none focus:border-indigo-500"
            />
          </div>

          <button
            type="submit"
            disabled={loading}
            className="w-full py-2.5 bg-indigo-600 hover:bg-indigo-500 disabled:opacity-50 text-white font-semibold rounded-lg text-sm transition-colors cursor-pointer mt-2"
          >
            {loading ? 'Kayıt Yapılıyor...' : 'Kayıt Ol'}
          </button>
        </form>

        {/* Yönlendirme */}
        <div className="text-center pt-2 border-t border-slate-800">
          <p className="text-xs text-slate-400">
            Zaten bir hesabınız var mı?{' '}
            <Link href="/login" className="text-indigo-400 hover:text-indigo-300 font-semibold transition-colors">
              Giriş Yap
            </Link>
          </p>
        </div>

      </div>
    </div>
  );

}