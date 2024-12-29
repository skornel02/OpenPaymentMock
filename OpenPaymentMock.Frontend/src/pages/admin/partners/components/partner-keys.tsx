import { Alert, AlertDescription, AlertTitle } from '@/components/ui/alert';
import { Button } from '@/components/ui/button';
import {
  Command,
  CommandEmpty,
  CommandGroup,
  CommandItem,
  CommandList,
} from '@/components/ui/command';
import { Input } from '@/components/ui/input';
import { Skeleton } from '@/components/ui/skeleton';
import { useAuthentication } from '@/contexts/authentication-context';
import { toast } from '@/hooks/use-toast';
import { SchemaPartnerShortDto } from '@/lib/openpaymentmock-backend';
import { client, useBackendQuery } from '@/lib/openpaymentmock-client';
import { CopyButton, Text } from '@mantine/core';
import { Check, Copy, Trash } from 'lucide-react';
import { DateTimePicker } from '@mantine/dates';
import { useState } from 'react';

export default function PartnerKeys({
  partner,
}: {
  partner: SchemaPartnerShortDto;
}) {
  const { apiKey } = useAuthentication();

  const [newAccessKey, setNewAccessKey] = useState<string | null>(null);
  const [accessKeyName, setAccessKeyName] = useState<string>('');
  const [accessKeyExpiresAt, setAccessKeyExpiresAt] =
    useState<DateValue | null>(null);

  const { data, isLoading, mutate } = useBackendQuery(
    '/api/partners/{id}/access-keys',
    {
      params: {
        path: {
          id: partner.id!,
        },
      },
      headers: {
        'X-Api-Key': apiKey,
      },
    },
  );

  const createNewKey = async () => {
    const { data } = await client.POST('/api/partners/{id}/access-keys', {
      params: {
        path: {
          id: partner.id!,
        },
      },
      body: {
        name: accessKeyName,
        expiresAt: accessKeyExpiresAt?.toISOString() ?? null,
      },
      headers: {
        'X-Api-Key': apiKey,
      },
    });

    if (data) {
      setNewAccessKey(data.key);
      mutate();
    } else {
      toast({
        title: 'Error creating access key',
        description: error.message,
        variant: 'destructive',
      });
    }
  };

  const deleteKey = async (accessKeyId: string) => {
    await client.DELETE('/api/partners/{id}/access-keys/{accessKeyId}', {
      params: {
        path: {
          id: partner.id!,
          accessKeyId,
        },
      },
      headers: {
        'X-Api-Key': apiKey,
      },
    });

    toast({
      title: 'Access key deleted',
      description: 'The access key has been deleted.',
    });

    mutate();
  };

  if (isLoading || !data) {
    return (
      <div className="h-[150px] w-full p-4">
        <Skeleton className="w-full h-full" />
      </div>
    );
  }

  return (
    <div className="p-4 flex flex-col">
      {newAccessKey && (
        <Alert>
          <Check className="h-4 w-4" />
          <AlertTitle>Access key created</AlertTitle>
          <AlertDescription>
            <p>
              Your access key is created and ready to use. Please store it in a
              safe!
            </p>
            <div className="flex flex-wrap">
              <p className="flex-grow bg-slate-200 mx-4 p-1 font-mono text-sm break-words contain-inline-size">
                {newAccessKey}
              </p>
              <CopyButton
                children={({ copy }) => (
                  <Button
                    onClick={() => {
                      copy();
                      toast({
                        title: 'Copied to clipboard',
                        description:
                          'The access key has been copied to your clipboard.',
                      });
                    }}
                  >
                    <Copy />
                  </Button>
                )}
                value={newAccessKey ?? ''}
              />
            </div>
          </AlertDescription>
        </Alert>
      )}
      <Command>
        <CommandList>
          <CommandGroup heading="Partner keys">
            <CommandEmpty> No access keys for this partner. </CommandEmpty>
            {data.map((accessKey) => (
              <CommandItem>
                <span className="text-lg mr-2">{accessKey.name}</span>
                <span>
                  Created at:{' '}
                  {new Date(accessKey.createdAt).toLocaleDateString()}
                </span>

                <span>
                  Expires at:{' '}
                  {accessKey.expiresAt
                    ? new Date(accessKey.expiresAt).toLocaleDateString()
                    : '-'}
                </span>
                <span>Deleted: {accessKey.deleted ? 'Yes' : 'No'}</span>
                <span>
                  Last used:{' '}
                  {accessKey.lastUsed
                    ? new Date(accessKey.lastUsed).toLocaleDateString()
                    : '-'}
                </span>
                <span>Usage count: {accessKey.usageCount}</span>
                <div className="flex-grow flex justify-end">
                  {!accessKey.deleted && (
                    <Button
                      variant="destructive"
                      onClick={() => deleteKey(accessKey.id)}
                    >
                      <Trash />
                    </Button>
                  )}
                </div>
              </CommandItem>
            ))}
          </CommandGroup>
        </CommandList>
      </Command>
      <div className="my-4 flex justify-end items-center gap-2 bg-white dark:bg-black p-2">
        <div className="flex-grow w-full flex flex-col">
          <p className="text-md font-semibold">Create new access key</p>
          <Input
            className="flex-grow"
            placeholder="Key name"
            value={accessKeyName}
            onChange={(e) => setAccessKeyName(e.target.value)}
          />
        </div>
        <DateTimePicker
          className="flex-grow w-full"
          defaultValue={null}
          clearable
          label="Expires at (optional)"
          value={accessKeyExpiresAt}
          onChange={(value) => setAccessKeyExpiresAt(value)}
        />
        <Button variant="secondary" onClick={createNewKey}>
          Generate new key
        </Button>
      </div>
      <div></div>
    </div>
  );
}
