import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import { Input } from '@/components/ui/input';
import { Label } from '@/components/ui/label';
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from '@/components/ui/select';
import { useAuthentication } from '@/contexts/authentication-context';
import { toast } from '@/hooks/use-toast';

import {
  SchemaPartnerShortDto,
  SchemaPaymentCreatedDto,
  SchemaPaymentSituationCreationDto,
} from '@/lib/openpaymentmock-backend';
import { client } from '@/lib/openpaymentmock-client';
import { Info } from 'lucide-react';
import { useState } from 'react';
import { Link } from 'react-router-dom';

export function AddPayment({
  partners,
  reload,
}: {
  partners: SchemaPartnerShortDto[];
  reload: () => void;
}) {
  const { apiKey } = useAuthentication();

  const [paymentCreated, setPaymentCreated] = useState<
    SchemaPaymentCreatedDto | undefined
  >();

  const [partnerId, setPartnerId] = useState<string | undefined>(undefined);

  const [form, setForm] = useState<Partial<SchemaPaymentSituationCreationDto>>({
    amount: 0,
    currency: 'HUF',
    callbackUrl: '',
    redirectUrl: '',
    secret: '',
    timeout: '00:10:00',
    paymentOptions: {
      allowInvalidCards: true,
      generateRandomCardDetails: true,
    },
  });

  const handleCreate = async () => {
    if (isNaN(form.amount!)) {
      toast({
        title: 'Error',
        description: 'Amount must be a number',
        variant: 'destructive',
      });
      return;
    }

    if (!form.currency) {
      toast({
        title: 'Error',
        description: 'Currency is required',
        variant: 'destructive',
      });
      return;
    }

    if (!form.callbackUrl) {
      toast({
        title: 'Error',
        description: 'Callback URL is required',
        variant: 'destructive',
      });
      return;
    }

    if (!form.redirectUrl) {
      toast({
        title: 'Error',
        description: 'Redirect URL is required',
        variant: 'destructive',
      });
      return;
    }

    if (!partnerId) {
      toast({
        title: 'Error',
        description: 'Partner is required',
        variant: 'destructive',
      });
      return;
    }

    try {
      const { data, error } = await client.POST('/api/payments', {
        params: {
          query: {
            partnerId: partnerId ?? '',
          },
        },
        headers: {
          'X-Api-Key': apiKey,
        },
        body: {
          amount: form.amount ?? 0,
          callbackUrl: form.callbackUrl ?? '',
          redirectUrl: form.redirectUrl ?? '',
          currency: form.currency ?? '',
          secret: form.secret ?? '',
          timeout: form.timeout ?? '00:10:00',
          paymentOptions: form.paymentOptions!,
        },
      });

      if (error) {
        toast({
          title: error.title ?? 'Error',
          description: error.detail,
          variant: 'destructive',
        });
      }

      if (data) {
        setPaymentCreated(data);
      }

      reload();
    } catch {
      toast({
        title: 'Error',
        description: "Couldn't create payment",
        variant: 'destructive',
      });
    }
  };

  return (
    <>
      <div className="grid grid-cols-4 gap-3 m-4">
        <div className="grid w-full max-w-sm items-center gap-1.5">
          <Label htmlFor="currency">Partner</Label>
          <Select value={partnerId} onValueChange={(val) => setPartnerId(val)}>
            <SelectTrigger className="min-w-[180px]">
              <SelectValue placeholder="Partner" />
            </SelectTrigger>
            <SelectContent>
              {partners.map((partner) => (
                <SelectItem key={partner.id} value={partner.id}>
                  {partner.name}
                </SelectItem>
              ))}
            </SelectContent>
          </Select>
        </div>
        <div className="grid w-full max-w-sm items-center gap-1.5">
          <Label htmlFor="currency">Currency</Label>
          <Input
            id="currency"
            value={form.currency}
            onChange={(e) =>
              setForm((val) => ({ ...val, currency: e.target.value }))
            }
          />
        </div>
        <div className="grid w-full max-w-sm items-center gap-1.5">
          <Label htmlFor="amount">Amount</Label>
          <Input
            id="amount"
            value={form.amount}
            onChange={(e) =>
              setForm((val) => ({ ...val, amount: Number(e.target.value) }))
            }
          />
        </div>
        <div className="grid w-full max-w-sm items-center gap-1.5">
          <Label htmlFor="callbackUrl">Callback URL</Label>
          <Input
            id="callbackUrl"
            value={form.callbackUrl}
            onChange={(e) =>
              setForm((val) => ({ ...val, callbackUrl: e.target.value }))
            }
          />
        </div>
        <div className="grid w-full max-w-sm items-center gap-1.5">
          <Label htmlFor="redirectUrl">Redirect URL</Label>
          <Input
            id="redirectUrl"
            value={form.redirectUrl}
            onChange={(e) =>
              setForm((val) => ({ ...val, redirectUrl: e.target.value }))
            }
          />
        </div>
        <div className="grid w-full max-w-sm items-center gap-1.5">
          <Label htmlFor="secret">Secret</Label>
          <Input
            id="secret"
            value={form.secret ?? undefined}
            onChange={(e) =>
              setForm((val) => ({ ...val, secret: e.target.value }))
            }
          />
        </div>
        <div className="grid w-full max-w-sm items-center gap-1.5">
          <Label htmlFor="timeout">Timeout</Label>
          <Input
            id="timeout"
            value={form.timeout}
            onChange={(e) =>
              setForm((val) => ({ ...val, timeout: e.target.value }))
            }
          />
        </div>
        <div className="grid w-full max-w-sm items-center gap-1.5">
          <Button onClick={handleCreate}>Create</Button>
        </div>
      </div>
      {paymentCreated && (
        <Alert className="max-w-lg mx-auto">
          <Info className="h-4 w-4" />
          <AlertTitle>Payment created</AlertTitle>
          <AlertDescription>
            Your payment situation was created.{' '}
            <Link
              target="_blank"
              to={paymentCreated?.redirectUrl ?? '#'}
              className="underline decoration-dashed"
            >
              Click here to open it.
            </Link>
          </AlertDescription>
        </Alert>
      )}
    </>
  );
}
