import { useCallback, useEffect, useMemo, useState } from "react";
import {
    MantineReactTable,
    MRT_Row,
    MRT_TableOptions,
    useMantineReactTable,
    type MRT_ColumnDef,
} from 'mantine-react-table';
import { SchemaPartnerShortDto } from "@/lib/openpaymentmock-backend";
import { client } from "@/lib/openpaymentmock-client";
import { useAuthentication } from "@/contexts/authentication-context";
import { Button } from "@/components/ui/button";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@radix-ui/react-tooltip";
import { Edit, Trash } from "lucide-react";

export default function PartnersPage() {
    const { apiKey } = useAuthentication();

    const [data, setData] = useState<SchemaPartnerShortDto[]>([]);
    const [isError, setIsError] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [isRefetching, setIsRefetching] = useState(false);

    const [validationErrors, setValidationErrors] = useState<
        Record<string, string | undefined>
    >({});

    const fetchData = useCallback(async () => {
        try {
            const { data } = await client.GET("/api/partners", {
                headers: {
                    "X-Api-Key": apiKey
                }
            });

            if (data) {
                setData(data);
            }
        } catch (error) {
            console.error(error);
            setIsError(true);
        }

        setIsLoading(false);
        setIsRefetching(false);
    }, [apiKey]);

    useEffect(() => {
        setIsLoading(true);
        fetchData();
    }, [fetchData]);

    const columns = useMemo<MRT_ColumnDef<SchemaPartnerShortDto>[]>(() => {
        const columns: MRT_ColumnDef<SchemaPartnerShortDto>[] = [
            {
                accessorKey: 'id',
                header: 'Id',
                enableEditing: false,
            },
            {
                accessorKey: 'name',
                header: 'Name',
                mantineEditTextInputProps: {
                    type: 'text',
                    required: true,
                    error: validationErrors?.name,
                    onFocus: () =>
                        setValidationErrors({
                            ...validationErrors,
                            name: undefined,
                        }),
                },
            },
        ];

        return columns;
    }, [validationErrors]);

    //CREATE action
    const handleCreateUser: MRT_TableOptions<SchemaPartnerShortDto>['onCreatingRowSave'] = async ({
        values,
        exitCreatingMode,
    }) => {
        console.log('Creating user', values);

        try {
            await client.POST("/api/partners", {
                headers: {
                    "X-Api-Key": apiKey
                },
                body: {
                    name: values.name,
                }
            });

            exitCreatingMode();
            fetchData();
        } catch (error) {
            console.error(error);
            setValidationErrors({
                name: 'Error creating user',
            });
        }
    };

    const openDeleteConfirmModal = async (row: MRT_Row<SchemaPartnerShortDto>) => {
        if (confirm('Are you sure you want to delete this user?')) {
            await client.DELETE("/api/partners/{id}", {
                headers: {
                    "X-Api-Key": apiKey
                },
                params: {
                    path: {
                        id: row.original.id ?? "-"
                    }
                }
            });

            fetchData();
        }
    }

    const table = useMantineReactTable({
        columns,
        data,
        getRowId: (row) => row.id,
        enableEditing: true,
        state: {
            isLoading,
            showProgressBars: isRefetching,
            showAlertBanner: isError,
        },
        mantineToolbarAlertBannerProps: isError
            ? { color: 'red', children: 'Error loading data' }
            : undefined,
        createDisplayMode: 'row',
        editDisplayMode: undefined,
        renderTopToolbarCustomActions: ({ table }) => (
            <div className="flex flex-row gap-2">
                <Button
                    variant="default"
                    onClick={() => {
                        table.setCreatingRow(true);
                    }}
                >
                    Add Partner
                </Button>
                <Button
                    variant="secondary"
                    onClick={() => {
                        setIsRefetching(true);
                        fetchData();
                    }}>
                    Refresh data
                </Button>
            </div>
        ),
        renderRowActions: ({ row, table }) => (
            <div className="flex flex-row gap-2">
                {/* <TooltipProvider>
                    <Tooltip>
                        <TooltipTrigger>
                            <Button
                                variant="outline"
                                onClick={() => table.setEditingRow(row)}
                            >
                                <Edit />
                            </Button>
                        </TooltipTrigger>
                        <TooltipContent>
                            Edit
                        </TooltipContent>
                    </Tooltip>
                </TooltipProvider> */}
                <TooltipProvider>
                    <Tooltip>
                        <TooltipTrigger>

                            <Button
                                variant="outline"
                                onClick={() => openDeleteConfirmModal(row)}
                            >
                                <Trash />
                            </Button>
                        </TooltipTrigger>
                        <TooltipContent>
                            Delete
                        </TooltipContent>
                    </Tooltip>
                </TooltipProvider>
            </div>
        ),
        onCreatingRowCancel: () => setValidationErrors({}),
        onCreatingRowSave: handleCreateUser,
        onEditingRowCancel: () => setValidationErrors({}),
    });

    return <MantineReactTable table={table} />;
}